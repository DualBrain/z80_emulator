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
                return _memory[address];
            }
            set
            {
                _memory[address] = value;
            }
        }

        public byte ReadByte(ushort address)
        {
            return this[address];
        }

        public ushort ReadWord(ushort address)
        {
            ushort val = this[address];
            val += (ushort) (this[address++] << 8);
            return val;
        }
    }
}
