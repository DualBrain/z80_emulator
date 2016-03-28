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

        public ushort HL
        {
            get { return (ushort) ((H << 8) + L); }
            set
            {
                L = (byte) (value & 0xFF);
                H = (byte) (value >> 8);
            }
        }

        public ushort BC
        {
            get { return (ushort) ((B << 8) + C); }
            set
            {
                C = (byte) (value & 0xFF);
                B = (byte) (value >> 8);
            }
        }

        public ushort DE
        {
            get { return (ushort) ((D << 8) + E); }
            set
            {
                E = (byte) (value & 0xFF);
                D = (byte) (value >> 8);
            }
        }

        public ushort IX { get; set; }
        public ushort IY { get; set; }
        public ushort PC { get; set; }
        public ushort SP { get; set; }

        public byte cary;

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
            HandleInstruction(nextByte);
        }

        private void HandleInstruction(byte instruction)
        {
            switch (instruction)
            {

                case 0x00:
                    //  NOP
                    break;
                case 0x01:
                    //  LD BC, nn
                    BC = Memory.ReadWord(PC);
                    PC += 2;
                    break;
                case 0x02:
                    //  LD (BC), A
                    Memory[BC] = A;
                    break;
                case 0x03:
                    //  INC BC
                    BC++;
                    break;
                case 0x04:
                    //  INC B
                    B++;
                    break;
                case 0x05:
                    //  DEC B
                    B--;
                    break;
                case 0x06:
                    //  LD B, n
                    B = FetchNextByte();
                    break;
                case 0x07:
                    //  RLCA
                    A = RotateLeft(A, 1);
                    break;
                case 0x08:
                    //  EX AF, AF'
                    var tmp = A;
                    A = A_Alt;
                    A_Alt = tmp;

                    tmp = F;
                    F = F_Alt;
                    F_Alt = tmp;
                    break;
                case 0x09:
                    //  ADD HL, BC
                    HL += BC;
                    break;
                case 0x0A:
                    //  LD A, (BC)
                    A = Memory[BC];
                    break;
                case 0x0B:
                    //  DEC BC
                    BC--;
                    break;
                case 0x0C:
                    //  INC C
                    C++;
                    break;
                case 0x0D:
                    //  DEC C
                    C--;
                    break;
                case 0x0E:
                    //  LD C, n
                    C = FetchNextByte();
                    break;
                case 0x0F:
                    //  RRCA
                    A = RotateRight(A, 1);
                    break;
                case 0x10:
                    //  DJNZ
                    //  TODO implement this
                    break;
                case 0x11:
                    //  LD DE, nn
                    DE = Memory.ReadWord(PC);
                    PC += 2;
                    break;
                case 0x12:
                    //  LD (DE), A
                    Memory[DE] = A;
                    break;
                case 0x13:
                    //  INC DE
                    DE++;
                    break;
                case 0x14:
                    //  INC D
                    D++;
                    break;
                case 0x15:
                    //  DEC D
                    D--;
                    break;
                case 0x16:
                    //  LD D, n
                    D = FetchNextByte();
                    break;
                case 0x17:
                    //  RLA
                    var val = CheckFlag(Flags.CF) ? 1 : 0;
                    SetFlag(Flags.CF, (val & 0x80) != 0);
                    A <<= 1;
                    A |= (byte)val;
                    break;
                case 0x18:
                    //  JR LABEL
                    sbyte e = (sbyte) Memory[PC];
                    PC = (ushort) (PC + e);
                    break;
                case 0x19:
                    //  ADD HL, DE
                    HL += DE;
                    break;
                case 0x1A:
                    //  LD A, (DE)
                    A = Memory[DE];
                    break;
                case 0x1B:
                    //  DEC DE
                    DE--;
                    break;
                case 0x1C:
                    //  INC E
                    E++;
                    break;
                case 0x1D:
                    //  DEC E
                    E--;
                    break;
                case 0x1E:
                    //  LD E, n
                    E = FetchNextByte();
                    break;
                case 0x1F:
                    //  RRA
                    val = CheckFlag(Flags.CF) ? 0x80 : 0;
                    SetFlag(Flags.CF, (val & 0x1) != 0);
                    A >>= 1;
                    A |= (byte)val;
                    break;
                case 0x20:
                    //  JR NZ LABEL
                    //  TODO implement this
                    break;
                case 0x21:
                    //  LD HL, nn
                    HL = Memory.ReadWord(PC);
                    PC += 2;
                    break;
                case 0x22:
                    //  LD (nn), HL
                    var address = Memory.ReadWord(PC);
                    PC += 2;
                    Memory[address] = L;
                    address++;
                    Memory[address] = H;
                    break;
                case 0x23:
                    //  INC HL
                    HL++;
                    break;
                case 0x24:
                    //  INC H
                    H++;
                    break;
                case 0x25:
                    //  DEC H
                    H--;
                    break;
                case 0x26:
                    // LD H, n
                    H = FetchNextByte();
                    break;
                case 0x27:
                    //  DAA
                    DAA();
                    break;
                case 0x28:
                    //  JR Z LABEL
                    //  TODO implement this
                    break;
                case 0x29:
                    //  ADD HL, HL
                    HL += HL;
                    break;
                case 0x2A:
                    //  LD HL, (nn)
                    address = Memory.ReadWord(PC);
                    PC += 2;
                    HL = Memory.ReadWord(address);
                    break;
                case 0x2B:
                    //  DEC HL
                    HL--;
                    break;
                case 0x2C:
                    //  INC L
                    L++;
                    break;
                case 0x2D:
                    //  DEC L
                    L--;
                    break;
                case 0x2E:
                    //  LD L, n
                    L = FetchNextByte();
                    break;
                case 0x2F:
                    //  CPL
                    A = (byte)~A;
                    break;
                case 0x30:
                    //  JR NC LABEL
                    //  TODO implement this
                    break;
                case 0x31:
                    //  LD SP, nn
                    SP = Memory.ReadWord(PC);
                    PC += 2;
                    break;
                case 0x32:
                    //  LD (nn), A
                    address = Memory.ReadWord(PC);
                    PC += 2;
                    Memory[address] = A;
                    break;
                case 0x33:
                    //  INC SP
                    SP++;
                    break;
                case 0x34:
                    //  INC (HL)
                    Memory[HL] = Memory[HL]++;
                    break;
                case 0x35:
                    //  DEC (HL)
                    Memory[HL] = Memory[HL]--;
                    break;
                case 0x36:
                    //  LD (HL), n
                    Memory[HL] = FetchNextByte();
                    break;
                case 0x37:
                    //  SCF
                    SetFlag(Flags.CF, true);
                    break;
                case 0x38:
                    //  JR C LABEL
                    //  TODO implement this
                    break;
                case 0x39:
                    //  ADD HL, SP
                    HL += SP;
                    break;
                case 0x3A:
                    //  LD A, (nn)
                    address = Memory.ReadWord(PC);
                    PC += 2;
                    A = Memory[address];
                    break;
                case 0x3B:
                    //  DEC SP
                    SP--;
                    break;
                case 0x3C:
                    //  INC A
                    A++;
                    break;
                case 0x3D:
                    //  DEC A
                    A--;
                    break;
                case 0x3E:
                    //  LD A, n
                    A = FetchNextByte();
                    break;
                case 0x3F:
                    //  CCF
                    SetFlag(Flags.CF, !CheckFlag(Flags.CF));
                    break;
                case 0x40:
                    //  LD B, B
                    B = B;
                    break;
                case 0x41:
                    //  LD B, C
                    B = C;
                    break;
                case 0x42:
                    //  LD B, D
                    B = D;
                    break;
                case 0x43:
                    //  LD B, E
                    B = E;
                    break;
                case 0x44:
                    //  LD B, H
                    B = H;
                    break;
                case 0x45:
                    //  LD B, L
                    B = L;
                    break;
                case 0x46:
                    //  LD B, (HL)
                    B = Memory[HL];
                    break;
                case 0x47:
                    //  LD B, A
                    B = A;
                    break;
                case 0x48:
                    //  LD C, B
                    C = B;
                    break;
                case 0x49:
                    //  LD C, C
                    C = C;
                    break;
                case 0x4A:
                    //  LD C, D
                    C = D;
                    break;
                case 0x4B:
                    //  LD C, E
                    C = E;
                    break;
                case 0x4C:
                    //  LD C, H
                    C = H;
                    break;
                case 0x4D:
                    //  LD C, L
                    C = L;
                    break;
                case 0x4E:
                    //  LD C, (HL)
                    C = Memory[HL];
                    break;
                case 0x4F:
                    //  LD C, A
                    C = A;
                    break;
                case 0x50:
                    //  LD D, B
                    D = B;
                    break;
                case 0x51:
                    //  LD D, C
                    D = C;
                    break;
                case 0x52:
                    //  LD D, D
                    D = D;
                    break;
                case 0x53:
                    //  LD D, E
                    D = E;
                    break;
                case 0x54:
                    //  LD D, H
                    D = H;
                    break;
                case 0x55:
                    //  LD D, L
                    D = L;
                    break;
                case 0x56:
                    //  LD D, (HL)
                    D = Memory[HL];
                    break;
                case 0x57:
                    //  LD D, A
                    D = A;
                    break;
                case 0x58:
                    //  LD E, B
                    E = B;
                    break;
                case 0x59:
                    //  LD E, C
                    E = C;
                    break;
                case 0x5A:
                    //  LD E, D
                    E = D;
                    break;
                case 0x5B:
                    //  LD E, E
                    E = E;
                    break;
                case 0x5C:
                    //  LD E, H
                    E = H;
                    break;
                case 0x5D:
                    //  LD E, L
                    E = L;
                    break;
                case 0x5E:
                    //  LD E, (HL)
                    E = Memory[HL];
                    break;
                case 0x5F:
                    //  LD E, A
                    E = A;
                    break;
                case 0x60:
                    //  LD H, B
                    H = B;
                    break;
                case 0x61:
                    //  LD H, C
                    H = C;
                    break;
                case 0x62:
                    //  LD H, D
                    H = D;
                    break;
                case 0x63:
                    //  LD H, E
                    H = E;
                    break;
                case 0x64:
                    //  LD H, H
                    H = H;
                    break;
                case 0x65:
                    //  LD H, L
                    H = L;
                    break;
                case 0x66:
                    //  LD H, (HL)
                    H = Memory[HL];
                    break;
                case 0x67:
                    //  LD H, A
                    H = A;
                    break;
                case 0x68:
                    //  LD L, B
                    L = B;
                    break;
                case 0x69:
                    //  LD L, C
                    L = C;
                    break;
                case 0x6A:
                    //  LD L, D
                    L = D;
                    break;
                case 0x6B:
                    //  LD L, E
                    L = E;
                    break;
                case 0x6C:
                    //  LD L, H
                    L = H;
                    break;
                case 0x6D:
                    //  LD L, L
                    L = L;
                    break;
                case 0x6E:
                    //  LD L, (HL)
                    L = Memory[HL];
                    break;
                case 0x6F:
                    L = A;
                    break;
                case 0x70:
                    Memory[HL] = B;
                    break;
                case 0x71:
                    Memory[HL] = C;
                    break;
                case 0x72:
                    Memory[HL] = D;
                    break;
                case 0x73:
                    Memory[HL] = E;
                    break;
                case 0x74:
                    Memory[HL] = H;
                    break;
                case 0x75:
                    Memory[HL] = L;
                    break;
                case 0x76:
                    HALT();
                    break;
                case 0x77:
                    Memory[HL] = A;
                    break;
                case 0x78:
                    A = B;
                    break;
                case 0x79:
                    A = C;
                    break;
                case 0x7A:
                    A = D;
                    break;
                case 0x7B:
                    A = E;
                    break;
                case 0x7C:
                    A = H;
                    break;
                case 0x7D:
                    A = L;
                    break;
                case 0x7E:
                    A = Memory[HL];
                    break;
                case 0x7F:
                    A = A;
                    break;
                case 0x80:
                    A = (byte) (A + B);
                    break;
                case 0x81:
                    A = (byte) (A + C);
                    break;
                case 0x82:
                    A = (byte) (A + D);
                    break;
                case 0x83:
                    A = (byte) (A + E);
                    break;
                case 0x84:
                    A = (byte) (A + H);
                    break;
                case 0x85:
                    A = (byte) (A + L);
                    break;
                case 0x86:
                    A = (byte) (A + HL);
                    break;
                case 0x87:
                    A = (byte) (A + A);
                    break;
                case 0x88:
                    A = (byte) (A + B + cary);
                    break;
                case 0x89:
                    A = (byte) (A + C + cary);
                    break;
                case 0x8A:
                    A = (byte) (A + D + cary);
                    break;
                case 0x8B:
                    A = (byte) (A + E + cary);
                    break;
                case 0x8C:
                    A = (byte) (A + H + cary);
                    break;
                case 0x8D:
                    A = (byte) (A + L + cary);
                    break;
                case 0x8E:
                    A = (byte) (A + HL + cary);
                    break;
                case 0x8F:
                    A = (byte) (A + A + cary);
                    break;
                case 0x90:
                    A = (byte) (A - B);
                    break;
                case 0x91:
                    A = (byte) (A - C);
                    break;
                case 0x92:
                    A = (byte) (A - D);
                    break;
                case 0x93:
                    A = (byte) (A - E);
                    break;
                case 0x94:
                    A = (byte) (A - H);
                    break;
                case 0x95:
                    A = (byte) (A - L);
                    break;
                case 0x96:
                    A = (byte) (A - HL);
                    break;
                case 0x97:
                    A = 0;
                    break;
                case 0x98:
                    A = (byte) (A - B - cary);
                    break;
                case 0x99:
                    A = (byte) (A - C - cary);
                    break;
                case 0x9A:
                    A = (byte) (A - D - cary);
                    break;
                case 0X9B:
                    A = (byte) (A - E - cary);
                    break;
                case 0x9C:
                    A = (byte) (A - H - cary);
                    break;
                case 0x9D:
                    A = (byte) (A - L - cary);
                    break;
                case 0x9E:
                    A = (byte) (A - HL - cary);
                    break;
                case 0x9F:
                    A = (byte) (A - A - cary);
                    break;
                case 0xA0:
                    A = (byte) (A & B);
                    break;
                case 0xA1:
                    A = (byte) (A & C);
                    break;
                case 0xA2:
                    A = (byte) (A & D);
                    break;
                case 0xA3:
                    A = (byte) (A & E);
                    break;
                case 0xA4:
                    A = (byte) (A & H);
                    break;
                case 0xA5:
                    A = (byte) (A & L);
                    break;
                case 0xA6:
                    A = (byte) (A & HL);
                    break;
                case 0xA7:
                    A = (byte) (A & A);
                    break;
                case 0xA8:
                    A = (byte) (A ^ B);
                    break;
                case 0xA9:
                    A = (byte) (A ^ C);
                    break;
                case 0xAA:
                    A = (byte) (A ^ D);
                    break;
                case 0xAB:
                    A = (byte) (A ^ E);
                    break;
                case 0xAC:
                    A = (byte) (A ^ H);
                    break;
                case 0xAD:
                    A = (byte) (A ^ L);
                    break;
                case 0xAE:
                    A = (byte) (A ^ HL);
                    break;
                case 0xAF:
                    A = (byte) (A ^ A);
                    break;
                case 0XB0:
                    A = (byte) (A | B);
                    break;
                case 0XB1:
                    A = (byte) (A ^ C);
                    break;
                case 0XB2:
                    A = (byte) (A ^ D);
                    break;
                case 0XB3:
                    A = (byte) (A ^ E);
                    break;
                case 0XB4:
                    A = (byte) (A ^ H);
                    break;
                case 0XB5:
                    A = (byte) (A ^ L);
                    break;
                case 0XB6:
                    A = (byte) (A ^ HL);
                    break;
                case 0XB7:
                    A = (byte) (A ^ A);
                    break;
                case 0XB8:
                    CP(B);
                    break;
                case 0XB9:
                    CP(C);
                    break;
                case 0XBA:
                    CP(D);
                    break;
                case 0XBB:
                    CP(E);
                    break;
                case 0XBC:
                    CP(H);
                    break;
                case 0XBD:
                    CP(L);
                    break;
                case 0XBE:
                    CP((byte) HL);
                    break;
                case 0XBF:
                    CP(A);
                    break;
                case 0xC0:
                    break;
                case 0xC1:
                    POP(BC);
                    break;
                case 0xC2:
                    break;
                case 0xC3:
                    break;
                case 0xC4:
                    break;
                case 0xC5:
                    PUSH(BC);
                    break;
                case 0xC6:
                    A += (byte) N;
                    break;
                case 0xC7:
                    break;
                case 0xC8:
                    break;
                case 0xC9:
                    break;
                case 0xCA:
                    break;
                case 0xCB: //TODO
                    break;
                case 0xCC:
                    break;
                case 0xCD:
                    break;
                case 0xCE:
                    A += (byte) ((byte) N + cary);
                    break;
                case 0xCF:
                    break;
                case 0xD0:
                    break;
                case 0xD1:
                    POP(DE);
                    break;
                case 0xD2:
                    break;
                case 0xD3:
                    OUT((byte) N, A);
                    break;
                case 0xD4:
                    break;
                case 0xD5:
                    PUSH(DE);
                    break;
                case 0xD6:
                    A -= (byte) N;
                    break;
                case 0xD7:
                    break;
                case 0xD8:
                    break;
                case 0xD9:
                    break;
                case 0xDA:
                    break;
                case 0xDB:
                    IN(A, (byte) N);
                    break;
                case 0xDC:
                    break;
                case 0xDD: //TODO
                    break;
                case 0xDE:
                    A -= (byte) ((byte) N - cary);
                    break;
                case 0xDF:
                    break;
                case 0xE0:
                    break;
                case 0xE1:
                    POP(HL);
                    break;
                case 0xE2:
                    break;
                case 0xE3:
                    break;
                case 0xE4:
                    break;
                case 0xE5:
                    PUSH(HL);
                    break;
                case 0xE6:
                    A = (byte) (A & (byte) N);
                    break;
                case 0xE7:
                    break;
                case 0xE8:
                    break;
                case 0xE9:
                    break;
                case 0xEA:
                    break;
                case 0xEB:
                    break;
                case 0xEC:
                    break;
                case 0xED:
                    Handle_ED();
                    break;
                case 0xEE:
                    A = (byte) (A ^ N);
                    break;
                case 0xEF:
                    break;
                case 0xF0:
                    break;
                case 0xF1:
                    break;
                case 0xF2:
                    break;
                case 0xF3:
                    break;
                case 0xF4:
                    break;
                case 0xF5:
                    break;
                case 0xF6:
                    A = (byte) (A | N);
                    break;
                case 0xF7:
                    break;
                case 0xF8:
                    break;
                case 0xF9:
                    break;
                case 0xFA:
                    break;
                case 0xFB:
                    break;
                case 0xFC:
                    break;
                case 0xFD:
                    Handle_FD();
                    break;
                case 0xFE:
                    break;
                case 0xFF:
                    break;
            }
        }

        private byte RotateRight(byte val, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                SetFlag(Flags.CF, (val & 0x80) != 0);
                val >>= 1;
                val |= CheckFlag(Flags.CF) ? (byte)0x80 : (byte)0;
            }

            return val;
        }

        private byte RotateLeft(byte val, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                SetFlag(Flags.CF, (val & 0x80) != 0);
                val <<= 1;
                val |= CheckFlag(Flags.CF) ? (byte) 1 : (byte) 0;
            }

            return val;
        }

        private void RRA()
        {
            throw new NotImplementedException();
        }

        private void DAA()
        {
            throw new NotImplementedException();
        }

        private void CPL()
        {
            throw new NotImplementedException();
        }

        private void SCF()
        {
            throw new NotImplementedException();
        }

        private void CCF()
        {
            throw new NotImplementedException();
        }

        private void HALT()
        {
            PC = 0;
            // TODO complete this
            throw new NotImplementedException();
        }

        private void CP(byte B)
        {
            throw new NotImplementedException();
        }

        private void IN(byte A, byte p)
        {
            throw new NotImplementedException();
        }

        private void OUT(byte p, byte A)
        {
            throw new NotImplementedException();
        }

        private void POP(ushort HL)
        {
            throw new NotImplementedException();
        }

        private void PUSH(ushort HL)
        {
            throw new NotImplementedException();
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
            return ((F >> (int) flag) & 0x1) == 1;
        }

        public void SetFlag(Flags flag, bool state)
        {
            byte x = (byte) (1 << (int) flag); // get the flag bit
            if (state)
                F |= x; //  set the flag
            else
                F &= (byte) (~x); //  reset the flag value
        }
    }
}