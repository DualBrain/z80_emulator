using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z80_Emulator
{
    public class Memory
    {
        private const ushort MEMORY_SIZE = 65535;   //  2^16 address lines
        private byte[] _memory;
        private int _romSize;

        public Memory(int romSize)
        {
            _memory = new byte[MEMORY_SIZE];
            _romSize = romSize;
        }

        public void LoadRom(byte[] rom)
        {
            if (rom.Length == 0)
                throw new ArgumentNullException(nameof(rom));

            if (rom.Length > _romSize)
                throw new InvalidOperationException("ROM file is larger than the available memory");

            rom.CopyTo(_memory, 0);
        }

        public byte this[ushort address]
        {
            get
            {
                CheckMemoryRange(address);
                return _memory[address];
            }
            set
            {
                CheckMemoryRange(address);
                _memory[address] = value;
            }
        }

        private void CheckMemoryRange(ushort address)
        {
            if (address < 0 || address > MEMORY_SIZE)
                throw new ArgumentOutOfRangeException(nameof(address));
        }
    }
}
