// Implementation of Musashi Motorola 68000 emulator as a Unity Native Plugin, in order to emulate original Amiga Battle Squadron musics
// Part of "gamejam dec 2020". This is quite hacky :)

#define _CRT_SECURE_NO_WARNINGS
#include <stdio.h>
#include <stdlib.h>
#include <assert.h>
#include <string.h>				// used for memcpy
#include "Musashi/m68k.h"

typedef	unsigned char u8;
typedef	unsigned short u16;
typedef	unsigned int u32;

// Because of native plugin & C# user land Paula emulation code, RAM will be always passed as an arg by any function here
// THIS PLUGIN IS OBVIOUSLY NOT THREAD SAFE! EVERYTHING SHOULD BE CALLED FROM MAIN THREAD ONLY
u32 gAmigaRamSize;
u8*	gAmigaRAM;
int	gExitCode;

int* gCustomWritePtr;
u32 gCustomWriteBufferSize;
u32 gCustomWriteBufferPos;

u16	swap16(u16 v)
{
	return (v << 8) | (v >> 8);
}

void	AmigaWriteCustomChips(int address, int value)
{
	if (gCustomWriteBufferPos > gCustomWriteBufferSize - 2)
	{
		gExitCode = 2;
		m68k_end_timeslice();
	}
	else
	{
		gCustomWritePtr[gCustomWriteBufferPos] = address;
		gCustomWritePtr[gCustomWriteBufferPos+1] = value;
		gCustomWriteBufferPos += 2;
	}
}

unsigned int  m68k_read_memory_16(unsigned int address)
{
	assert(address <= gAmigaRamSize - 2);
	const u16* r = (const u16*)(gAmigaRAM + address);
	return swap16(*r);
}

unsigned int  m68k_read_memory_8(unsigned int address)
{
	assert(address <= gAmigaRamSize - 2);
	const u8* r = (const u8*)(gAmigaRAM + address);
	return *r;
}

unsigned int  m68k_read_memory_32(unsigned int address)
{
	assert(address <= gAmigaRamSize - 4);
	unsigned int v0 = m68k_read_memory_16(address);
	unsigned int v1 = (m68k_read_memory_16(address + 2)) & 0xffff;
	return (v0 << 16) | v1;
}

void m68k_write_memory_8(unsigned int address, unsigned int value)
{
	if ((address >= 0xbfd000) && (address <= 0xbfef01))			// amiga CIA timer things
		AmigaWriteCustomChips(address, value);
	else
	{
		assert(address <= gAmigaRamSize - 1);
		u8* r = (u8*)(gAmigaRAM + address);
		*r = u8(value);
	}
}

void m68k_write_memory_16(unsigned int address, unsigned int value)
{
	if ((address >= 0xdff000) && (address < 0xdff200))	// amiga standard custom chips
		AmigaWriteCustomChips(address, value);
	else
	{
		assert(address <= gAmigaRamSize - 2);
		u16* r = (u16*)(gAmigaRAM + address);
		*r = swap16(u16(value));						// warning: store in host ram using little endian
	}
}

void m68k_write_memory_32(unsigned int address, unsigned int value)
{
	m68k_write_memory_16(address, value >> 16);
	m68k_write_memory_16(address + 2, value & 0xffff);
}

static int	fIllegalCb(int opcode)
{
	assert(false);
	m68k_end_timeslice();
	return 1;
}

static void	fResetCb(void)
{
	gExitCode = 0;
	m68k_end_timeslice();
}

// m68k C# API
#if defined(_WIN32)
    #define EXPORTDLL __declspec(dllexport)
#elif defined(__APPLE__) || defined(LINUX)
    #define EXPORTDLL __attribute__((visibility("default")))
#else
    #define EXPORTDLL
#endif

extern "C"
{

	EXPORTDLL int m68kInit(void)
	{
		m68k_set_cpu_type(M68K_CPU_TYPE_68000);
		m68k_init();
		m68k_set_illg_instr_callback(fIllegalCb);
		m68k_set_reset_instr_callback(fResetCb);
		return 0;
	}

	EXPORTDLL int m68kUpload(u8* ram, int ramSize, const u8* data, int dataSize, int uploadAd)
	{
		gAmigaRAM = ram;
		gAmigaRamSize = u32(ramSize);

		u32 uploadAdEnd = uploadAd + dataSize;
		if (uploadAdEnd > gAmigaRamSize)
			return 1;		// out of memory bound

		memcpy(ram + uploadAd, data, dataSize);
		return 0;
	}

	// Main function: call a 68k code and fill a table with all Amiga custom chips write
	EXPORTDLL int m68kCall(u8* ram, int ramSize, int callAddress, int& customChipWriteCount, int* customChipWriteBuffer, int customChipWriteBufferSize)
	{
		gAmigaRAM = ram;
		gAmigaRamSize = u32(ramSize);

		gCustomWritePtr = customChipWriteBuffer;
		gCustomWriteBufferSize = customChipWriteBufferSize;
		gCustomWriteBufferPos = 0;

		// Call a subroutine in Amiga address
		// Small hack to return to C after RTS: putting "reset" instruction address in the stack, so when RTS occurs, it jumps to reset instruction
		m68k_write_memory_16(0x1000, 0x4e70);			// write reset instruction at $1000 address
		m68k_write_memory_32(gAmigaRamSize - 4, 0x1000);		// next RTS will jump to $1000
		m68k_write_memory_32(0, gAmigaRamSize - 4);		// stack ptr at top ( $0 is the stack pointer after 68k reset)
		m68k_write_memory_32(4, callAddress);							// pc after reset
		m68k_pulse_reset();

		int timeOutInVblank = 50;
		gExitCode = 1;
		int cycles = 0;
		for (int t = 0; t < timeOutInVblank; t++)
		{
			cycles += m68k_execute(512 * 313);				// 50hz frame
			if (gExitCode != 1)
				break;
		}

		customChipWriteCount = gCustomWriteBufferPos;
		return gExitCode;
	}

}
