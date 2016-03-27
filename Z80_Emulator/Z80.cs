using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z80_Emulator
{
    public class Z80
    {
        public enum Flags
        {
            CF = 1,
            N = 2,
            P_V = 4,
            H = 16,
            Z = 32,
            S = 128,
        }

        public byte A { get; set; }
        public byte F { get; set; }
        public byte A_Alt { get; set; }
        public byte F_Alt { get; set; }
        public byte B { get; set; }
        public byte C { get; set; }
        public byte B_Alt { get; set; }
        public byte C_Alt { get; set; }
        public byte D { get; set; }
        public byte E { get; set; }
        public byte D_Alt { get; set; }
        public byte E_Alt { get; set; }
        public byte H { get; set; }
        public byte L { get; set; }
        public byte H_Alt { get; set; }
        public byte L_Alt { get; set; }

        public ushort HL => (ushort)(L + (H << 8));

        public ushort BC
        {
            get
            {
                return (ushort)(B << 8) + C);
            }
            set
            {
                C = (byte)(value & 0xFF);
                B = (byte)(value >> 8);
            }
        }
        public ushort DE => (ushort)(E + (D << 8));

        public ushort IX { get; set; }
        public ushort IY { get; set; }
        public ushort PC { get; set; }
        public ushort SP { get; set; }

        public Memory Memory { get; private set; }

        public Z80(Memory memory)
        {
            if (memory == null)
                throw new ArgumentNullException(nameof(memory));

            Memory = memory;
        }

        public void StepEmulation()
        {
            var nextByte = FetchNextByte();

            if (nextByte == 0xCB)
            {
                // TODO complete this
                Handle_CB();
            }
            else if (nextByte == 0xED)
            {
                // TODO complete this
                Handle_ED();
            }
            else if (nextByte == 0xDD)
            {
                // TODO complete this
                Handle_DD();
            }
            else if (nextByte == 0xFD)
            {
                // TODO complete this
                Handle_FD();
            }
            else
            {
                Handle_Normal(nextByte);
            }
        }

        private void Handle_Normal(byte op)
        {
            switch (op)
            {
                case 0x00:
                    break;
                case 0x01:
                    break;
            }
        }

        private void Handle_CB()
        {
            throw new NotImplementedException();
        }

        private void Handle_ED()
        {
            throw new NotImplementedException();
        }

        private void Handle_DD()
        {
            throw new NotImplementedException();
        }

        private void Handle_FD()
        {
            throw new NotImplementedException();
        }

        private byte FetchNextByte()
        {
            var val = Memory[PC];
            PC++;

            return val;
        }
        
        public bool CheckFlag(Flags flag)
        {
            return ((F >> (int)flag) & 0x1) == 1;
        }

        public void SetFlag(Flags flag, bool state)
        {
            byte x = (byte)(1 << (int)flag);    // get the flag bit
            if (state)
                F |= x;             //  set the flag
            else
                F &= (byte)(~x);    //  reset the flag value
        }

        public void Reset()
        {
            PC = 0;
            // TODO complete this
        }
    }
}
