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
        public byte IXh { get; set; }
        public byte IXl{ get; set; }

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

        public ushort IX
        {
            get { return (ushort)((IXh << 8) + IXl); }
            set
            {
                IXl = (byte)(value & 0xFF);
                IXh = (byte)(value >> 8);
            }
        }
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
                    A = RLC(A, 1);
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
                    A = RRC(A, 1);
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
                    //  LD L, A
                    L = A;
                    break;
                case 0x70:
                    //  LD (HL), B
                    Memory[HL] = B;
                    break;
                case 0x71:
                    //  LD (HL), C
                    Memory[HL] = C;
                    break;
                case 0x72:
                    //  LD (HL), D
                    Memory[HL] = D;
                    break;
                case 0x73:
                    //  LD (HL), E
                    Memory[HL] = E;
                    break;
                case 0x74:
                    //  LD (HL), H
                    Memory[HL] = H;
                    break;
                case 0x75:
                    //  LD (HL), L
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
                    //A += (byte) N;
                    break;
                case 0xC7:
                    break;
                case 0xC8:
                    break;
                case 0xC9:
                    break;
                case 0xCA:
                    break;
                case 0xCB: Handle_CB();
                    break;
                case 0xCC:
                    break;
                case 0xCD:
                    break;
                case 0xCE:
                    //A += (byte) ((byte) N + cary);
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
                    //OUT((byte) N, A);
                    break;
                case 0xD4:
                    break;
                case 0xD5:
                    PUSH(DE);
                    break;
                case 0xD6:
                    //A -= (byte) N;
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
                    //IN(A, (byte) N);
                    break;
                case 0xDC:
                    break;
                case 0xDD: //TODO
                    break;
                case 0xDE:
                    //A -= (byte) ((byte) N - cary);
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
                    //A = (byte) (A & (byte) N);
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
                    //A = (byte) (A ^ N);
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
                    //A = (byte) (A | N);
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

        private void Handle_CB()
        {
            byte b = FetchNextByte();
            switch(b)
            {
                case 0x00: RLC(B, 1);
                    break;
                case 0x01: RLC(C, 1);
                    break;
                case 0x02: RLC(D, 1);
                    break;
                case 0x03: RLC(E, 1);
                    break;
                case 0x04: RLC(H, 1);
                    break;
                case 0x05: RLC(L, 1);
                    break;
                case 0x06: RLC(Memory[HL],1 );
                    break;
                case 0x07: RLC(A, 1);
                    break;
                case 0x08: RRC(B, 1);
                    break;
                case 0x09: RRC(C, 1);
                    break;
                case 0x0A: RRC(D, 1);
                    break;
                case 0x0B: RRC(E, 1);
                    break;
                case 0x0C: RRC(H, 1);
                    break;
                case 0x0D: RRC(L, 1);
                    break;
                case 0x0E: RRC(Memory[HL], 1);
                    break;
                case 0x0F: RRC(A, 1);
                    break;
                case 0x10: RL(B, 1);
                    break;
                case 0x11: RL(C, 1);
                    break;
                case 0x12: RL(D, 1);
                    break;
                case 0x13: RL(E, 1);
                    break;
                case 0x14: RL(H, 1);
                    break;
                case 0x15: RL(L, 1);
                    break;
                case 0x16: RL(Memory[HL], 1);
                    break;
                case 0x17: RL(A, 1);
                    break;
                case 0x18: RR(B, 1);
                    break;
                case 0x19: RR(C, 1);
                    break;
                case 0x1A: RR(D, 1);
                    break;
                case 0x1B: RR(E, 1);
                    break;
                case 0x1C: RR(H, 1);
                    break;
                case 0x1D: RR(L, 1);
                    break;
                case 0x1E: RR(Memory[HL], 1);
                    break;
                case 0x1F: RR(A, 1);
                    break;
                case 0x20: SLA(B, 1);
                    break;
                case 0x21: SLA(C, 1);
                    break;
                case 0x22: SLA(D, 1);
                    break;
                case 0x23: SLA(E, 1);
                    break;
                case 0x24: SLA(H, 1);
                    break;
                case 0x25: SLA(L, 1);
                    break;
                case 0x26: SLA(Memory[HL], 1);
                    break;
                case 0x27: SLA(A, 1);
                    break;
                case 0x28: SRA(B, 1);
                    break;
                case 0x29: SRA(C, 1);
                    break;
                case 0x2A: SRA(D, 1);
                    break;
                case 0x2B: SRA(E, 1);
                    break;
                case 0x2C: SRA(H, 1);
                    break;
                case 0x2D: SRA(L, 1);
                    break;
                case 0x2E: SRA(Memory[HL], 1);
                    break;
                case 0x2F: SRA(A, 1);
                    break;
                case 0x30: SLL(B, 1);
                    break;
                case 0x31: SLL(C, 1);
                    break;
                case 0x32: SLL(D, 1);
                    break;
                case 0x33: SLL(E, 1);
                    break;
                case 0x34: SLL(H, 1);
                    break;
                case 0x35: SLL(L, 1);
                    break;
                case 0x36: SLL(Memory[HL], 1);
                    break;
                case 0x37: SLL(A, 1);
                    break;
                case 0x38: SRL(B, 1);
                    break;
                case 0x39: SRL(C, 1);
                    break;
                case 0x3A: SRL(D, 1);
                    break;
                case 0x3B: SRL(E, 1);
                    break;
                case 0x3C: SRL(H, 1);
                    break;
                case 0x3D: SRL(L, 1);
                    break;
                case 0x3E: SRL(Memory[HL], 1);
                    break;
                case 0x3F: SRL(A, 1);
                    break;
                case 0x40: BIT(B, 0);          //bit0
                    break;
                case 0x41: BIT(C, 0);
                    break;
                case 0x42: BIT(D, 0);
                    break;
                case 0x43: BIT(E, 0);
                    break;
                case 0x44: BIT(H, 0);
                    break;
                case 0x45: BIT(L, 0);
                    break;
                case 0x46: BIT(Memory[HL], 0);
                    break;
                case 0x47: BIT(A, 0);          //end
                    break;
                case 0x48: BIT(B, 1);          //bit1
                    break;
                case 0x49: BIT(C, 1);
                    break;
                case 0x4A: BIT(D, 1);
                    break;
                case 0x4B: BIT(E, 1);
                    break;
                case 0x4C: BIT(H, 1);
                    break;
                case 0x4D: BIT(L, 1);
                    break;
                case 0x4E: BIT(Memory[HL], 1);
                    break;
                case 0x4F: BIT(A, 1);          //end
                    break;
                case 0x50: BIT(B, 2);          //bit2
                    break;
                case 0x51: BIT(C, 2);
                    break;
                case 0x52: BIT(D, 2);
                    break;
                case 0x53: BIT(E, 2);
                    break;
                case 0x54: BIT(H, 2);
                    break;
                case 0x55: BIT(L, 2);
                    break;
                case 0x56: BIT(Memory[HL], 2);
                    break;
                case 0x57: BIT(A, 2);          //end
                    break;
                case 0x58: BIT(B, 3);          //bit3
                    break;
                case 0x59: BIT(C, 3);
                    break;
                case 0x5A: BIT(D, 3);
                    break;
                case 0x5B: BIT(E, 3);
                    break;
                case 0x5C: BIT(H, 3);
                    break;
                case 0x5D: BIT(L, 3);
                    break;
                case 0x5E: BIT(Memory[HL], 3);
                    break;
                case 0x5F: BIT(A, 3);           //end
                    break;
                case 0x60: BIT(B, 4);
                    break;
                case 0x61: BIT(C, 4);
                    break;
                case 0x62: BIT(D, 4);
                    break;
                case 0x63: BIT(E, 4);
                    break;
                case 0x64: BIT(H, 4);
                    break;
                case 0x65: BIT(L, 4);
                    break;
                case 0x66: BIT(Memory[HL], 4);
                    break;
                case 0x67: BIT(A, 4);
                    break;
                case 0x68: BIT(B, 5);
                    break;
                case 0x69: BIT(C, 5);
                    break;
                case 0x6A: BIT(D, 5);
                    break;
                case 0x6B: BIT(E, 5);
                    break;
                case 0x6C: BIT(H, 5);
                    break;
                case 0x6D: BIT(L, 5);
                    break;
                case 0x6E: BIT(Memory[HL], 5);
                    break;
                case 0x6F: BIT(A, 5);
                    break;
                case 0x70: BIT(B, 6);
                    break;
                case 0x71: BIT(C, 6);
                    break;
                case 0x72: BIT(D, 6);
                    break;
                case 0x73: BIT(E, 6);
                    break;
                case 0x74: BIT(H, 6);
                    break;
                case 0x75: BIT(L, 6);
                    break;
                case 0x76: BIT(Memory[HL], 6);
                    break;
                case 0x77: BIT(A, 6);
                    break;
                case 0x78: BIT(B, 7);
                    break;
                case 0x79: BIT(C, 7);
                    break;
                case 0x7A: BIT(D, 7);
                    break;
                case 0x7B: BIT(E, 7);
                    break;
                case 0x7C: BIT(H, 7);
                    break;
                case 0x7D: BIT(L, 7);
                    break;
                case 0x7E: BIT(Memory[HL], 7);
                    break;
                case 0x7F: BIT(A, 7);
                    break;
                case 0x80: RES(B, 0);
                    break;
                case 0x81: RES(C, 0);
                    break;
                case 0x82: RES(D, 0);
                    break;
                case 0x83: RES(E, 0);
                    break;
                case 0x84: RES(H, 0);
                    break;
                case 0x85: RES(L, 0);
                    break;
                case 0x86: RES(Memory[HL], 0);
                    break;
                case 0x87: RES(A, 0);
                    break;
                case 0x88: RES(B, 1);
                    break;
                case 0x89: RES(C, 1);
                    break;
                case 0x8A: RES(D, 1);
                    break;
                case 0x8B: RES(E, 1);
                    break;
                case 0x8C: RES(H, 1);
                    break;
                case 0x8D: RES(L, 1);
                    break;
                case 0x8E: RES(Memory[HL], 1);
                    break;
                case 0x8F: RES(A, 1);
                    break;
                case 0x90: RES(B, 2);
                    break;
                case 0x91: RES(C, 2);
                    break;
                case 0x92: RES(D, 2);
                    break;
                case 0x93: RES(E, 2);
                    break;
                case 0x94: RES(H, 2);
                    break;
                case 0x95: RES(L, 2);
                    break;
                case 0x96: RES(Memory[HL], 2);
                    break;
                case 0x97: RES(A, 2);
                    break;
                case 0x98: RES(B, 3);
                    break;
                case 0x99: RES(C, 3);
                    break;
                case 0x9A: RES(D, 3);
                    break;
                case 0X9B: RES(E, 3);
                    break;
                case 0x9C: RES(H, 3);
                    break;
                case 0x9D: RES(L, 3);
                    break;
                case 0x9E: RES(Memory[HL], 3);
                    break;
                case 0x9F: RES(A, 3);
                    break;
                case 0xA0: RES(B, 4);
                    break;
                case 0xA1: RES(C, 4);
                    break;
                case 0xA2: RES(D, 4);
                    break;
                case 0xA3: RES(E, 4);
                    break;
                case 0xA4: RES(H, 4);
                    break;
                case 0xA5: RES(L, 4);
                    break;
                case 0xA6: RES(Memory[HL], 4);
                    break;
                case 0xA7: RES(A, 4);
                    break;
                case 0xA8: RES(B, 5);
                    break;
                case 0xA9: RES(C, 5);
                    break;
                case 0xAA: RES(D, 5);
                    break;
                case 0xAB: RES(E, 5);
                    break;
                case 0xAC: RES(H, 5);
                    break;
                case 0xAD: RES(L, 5);
                    break;
                case 0xAE: RES(Memory[HL], 5);
                    break;
                case 0xAF: RES(A, 5);
                    break;
                case 0XB0: RES(B, 6);
                    break;
                case 0XB1: RES(C, 6);
                    break;
                case 0XB2: RES(D, 6);
                    break;
                case 0XB3: RES(E, 6);
                    break;
                case 0XB4: RES(H, 6);
                    break;
                case 0XB5: RES(L, 6);
                    break;
                case 0XB6: RES(Memory[HL], 6);
                    break;
                case 0XB7: RES(A, 6);
                    break;
                case 0XB8: RES(B, 7);
                    break;
                case 0XB9: RES(C, 7);
                    break;
                case 0XBA: RES(D, 7);
                    break;
                case 0XBB: RES(E, 7);
                    break;
                case 0XBC: RES(H, 7);
                    break;
                case 0XBD: RES(L, 7);
                    break;
                case 0XBE: RES(Memory[HL], 7);
                    break;
                case 0XBF: RES(A, 7);
                    break;
                case 0xC0: SET(B, 0);
                    break;
                case 0xC1: SET(C, 0);
                    break;
                case 0xC2: SET(D, 0);
                    break;
                case 0xC3: SET(E, 0);
                    break;
                case 0xC4: SET(H, 0);
                    break;
                case 0xC5: SET(L, 0);
                    break;
                case 0xC6: SET(Memory[HL], 0);
                    break;
                case 0xC7: SET(A, 0);
                    break;
                case 0xC8: SET(B, 1);
                    break;
                case 0xC9: SET(C, 1);
                    break;
                case 0xCA: SET(D, 1);
                    break;
                case 0xCB: SET(E, 1);
                    break;
                case 0xCC: SET(H, 1);
                    break;
                case 0xCD: SET(L, 1);
                    break;
                case 0xCE: SET(Memory[HL], 1);
                    break;
                case 0xCF: SET(A, 1);
                    break;
                case 0xD0: SET(B, 2);
                    break;
                case 0xD1: SET(C, 2);
                    break;
                case 0xD2: SET(D, 2);
                    break;
                case 0xD3: SET(E, 2);
                    break;
                case 0xD4: SET(H, 2);
                    break;
                case 0xD5: SET(L, 2);
                    break;
                case 0xD6: SET(Memory[HL], 2);
                    break;
                case 0xD7: SET(A, 2);
                    break;
                case 0xD8: SET(B, 3);
                    break;
                case 0xD9: SET(C, 3);
                    break;
                case 0xDA: SET(D, 3);
                    break;
                case 0xDB: SET(E, 3);
                    break;
                case 0xDC: SET(H, 3);
                    break;
                case 0xDD: SET(L, 3);
                    break;
                case 0xDE: SET(Memory[HL], 3);
                    break;
                case 0xDF: SET(A, 3);
                    break;
                case 0xE0: SET(B, 4);
                    break;
                case 0xE1: SET(C, 4);
                    break;
                case 0xE2: SET(D, 4);
                    break;
                case 0xE3: SET(E, 4);
                    break;
                case 0xE4: SET(H, 4);
                    break;
                case 0xE5: SET(L, 4);
                    break;
                case 0xE6: SET(Memory[HL], 4);
                    break;
                case 0xE7: SET(A, 4);
                    break;
                case 0xE8: SET(B, 5);
                    break;
                case 0xE9: SET(C, 5);
                    break;
                case 0xEA: SET(D, 5);
                    break;
                case 0xEB: SET(E, 5);
                    break;
                case 0xEC: SET(H, 5);
                    break;
                case 0xED: SET(L, 5);
                    break;
                case 0xEE: SET(Memory[HL], 5);
                    break;
                case 0xEF: SET(A, 5);
                    break;
                case 0xF0: SET(B, 6);
                    break;
                case 0xF1: SET(C, 6);
                    break;
                case 0xF2: SET(D, 6);
                    break;
                case 0xF3: SET(E, 6);
                    break;
                case 0xF4: SET(H, 6);
                    break;
                case 0xF5: SET(L, 6);
                    break;
                case 0xF6: SET(Memory[HL], 6);
                    break;
                case 0xF7: SET(A, 6);
                    break;
                case 0xF8: SET(B, 7);
                    break;
                case 0xF9: SET(C, 7);
                    break;
                case 0xFA: SET(D, 7);
                    break;
                case 0xFB: SET(E, 7);
                    break;
                case 0xFC: SET(H, 7);
                    break;
                case 0xFD: SET(L, 7);
                    break;
                case 0xFE: SET(Memory[HL], 7);
                    break;
                case 0xFF: SET(A, 7);
                    break;
            }
            
        }

        private void Handle_DD()
        {
            byte b = FetchNextByte();
            switch (b)
            {
                case 0x00:break;
                case 0x01:break;
                case 0x02:break;
                case 0x03:break;
                case 0x04:break;
                case 0x05:break;
                case 0x06:break;
                case 0x07:break;
                case 0x08:break;
                case 0x09:break;
                case 0x0A: IX += BC; break;
                case 0x0B:break;
                case 0x0C:break;
                case 0x0D:break;
                case 0x0E:break;
                case 0x0F:break;
                case 0x10:break;
                case 0x11:break;
                case 0x12:break;
                case 0x13:break;
                case 0x14:break;
                case 0x15:break;
                case 0x16:break;
                case 0x17:break;
                case 0x18:break;
                case 0x19: IX += DE;break;
                case 0x1A:break;
                case 0x1B:break;
                case 0x1C:break;
                case 0x1D:break;
                case 0x1E:break;
                case 0x1F:break;
                case 0x20:break;
                case 0x21: IX = FetchNextWord(); 
                    break;
                case 0x22: ushort temp = FetchNextWord(); Memory[temp] = (byte)(IX); Memory[++temp] = (byte)(IX >> 8); PC++; // ex. temp = 45f5H , IX = 459fH , 
                    break;                                                                                            //Memory[45f5] = 9fH , Memory[45f6] = 45H , PC = PC + 2
                case 0x23: IX++;
                    break;
                case 0x24: IXh++; ;
                    break;
                case 0x25: IXl--;
                    break;
                case 0x26: IXh--;
                    break;
                case 0x27: IXh = FetchNextByte();
                    break;
                case 0x28:break;
                case 0x29:break;
                case 0x2A: IX += IX;
                    break;
                case 0x2B: IX = Memory[FetchNextWord()];
                    break;
                case 0x2C: IX--;
                    break;
                case 0x2D: IXl++;
                    break;
                case 0x2E: IXl--;
                    break;
                case 0x2F: IXl = FetchNextByte();
                    break;
                case 0x30:
                    break;
                case 0x31:
                    break;
                case 0x32:
                    break;
                case 0x33:
                    break;
                case 0x34:
                    break;
                case 0x35:
                    break;
                case 0x36:
                    break;
                case 0x37:
                    break;
                case 0x38:
                    break;
                case 0x39:
                    break;
                case 0x3A:
                    break;
                case 0x3B:
                    break;
                case 0x3C:
                    break;
                case 0x3D:
                    break;
                case 0x3E:
                    break;
                case 0x3F:
                    break;
                case 0x40:
                    break;
                case 0x41:
                    break;
                case 0x42:
                    break;
                case 0x43:
                    break;
                case 0x44:
                    break;
                case 0x45:
                    break;
                case 0x46:
                    break;
                case 0x47:
                    break;
                case 0x48:
                    break;
                case 0x49:
                    break;
                case 0x4A:
                    break;
                case 0x4B:
                    break;
                case 0x4C:
                    break;
                case 0x4D:
                    break;
                case 0x4E:
                    break;
                case 0x4F:
                    break;
                case 0x50:
                    break;
                case 0x51:
                    break;
                case 0x52:
                    break;
                case 0x53:
                    break;
                case 0x54:
                    break;
                case 0x55:
                    break;
                case 0x56:
                    break;
                case 0x57:
                    break;
                case 0x58:
                    break;
                case 0x59:
                    break;
                case 0x5A:
                    break;
                case 0x5B:
                    break;
                case 0x5C:
                    break;
                case 0x5D:
                    break;
                case 0x5E:
                    break;
                case 0x5F:
                    break;
                case 0x60:
                    break;
                case 0x61:
                    break;
                case 0x62:
                    break;
                case 0x63:
                    break;
                case 0x64:
                    break;
                case 0x65:
                    break;
                case 0x66:
                    break;
                case 0x67:
                    break;
                case 0x68:
                    break;
                case 0x69:
                    break;
                case 0x6A:
                    break;
                case 0x6B:
                    break;
                case 0x6C:
                    break;
                case 0x6D:
                    break;
                case 0x6E:
                    break;
                case 0x6F:
                    break;
                case 0x70:
                    break;
                case 0x71:
                    break;
                case 0x72:
                    break;
                case 0x73:
                    break;
                case 0x74:
                    break;
                case 0x75:
                    break;
                case 0x76:
                    break;
                case 0x77:
                    break;
                case 0x78:
                    break;
                case 0x79:
                    break;
                case 0x7A:
                    break;
                case 0x7B:
                    break;
                case 0x7C:
                    break;
                case 0x7D:
                    break;
                case 0x7E:
                    break;
                case 0x7F:
                    break;
                case 0x80:
                    break;
                case 0x81:
                    break;
                case 0x82:
                    break;
                case 0x83:
                    break;
                case 0x84:
                    break;
                case 0x85:
                    break;
                case 0x86:
                    break;
                case 0x87:
                    break;
                case 0x88:
                    break;
                case 0x89:
                    break;
                case 0x8A:
                    break;
                case 0x8B:
                    break;
                case 0x8C:
                    break;
                case 0x8D:
                    break;
                case 0x8E:
                    break;
                case 0x8F:
                    break;
                case 0x90:
                    break;
                case 0x91:
                    break;
                case 0x92:
                    break;
                case 0x93:
                    break;
                case 0x94:
                    break;
                case 0x95:
                    break;
                case 0x96:
                    break;
                case 0x97:
                    break;
                case 0x98:
                    break;
                case 0x99:
                    break;
                case 0x9A:
                    break;
                case 0X9B:
                    break;
                case 0x9C:
                    break;
                case 0x9D:
                    break;
                case 0x9E:
                    break;
                case 0x9F:
                    break;
                case 0xA0:
                    break;
                case 0xA1:
                    break;
                case 0xA2:
                    break;
                case 0xA3:
                    break;
                case 0xA4:
                    break;
                case 0xA5:
                    break;
                case 0xA6:
                    break;
                case 0xA7:
                    break;
                case 0xA8:
                    break;
                case 0xA9:
                    break;
                case 0xAA:
                    break;
                case 0xAB:
                    break;
                case 0xAC:
                    break;
                case 0xAD:
                    break;
                case 0xAE:
                    break;
                case 0xAF:
                    break;
                case 0XB0:
                    break;
                case 0XB1:
                    break;
                case 0XB2:
                    break;
                case 0XB3:
                    break;
                case 0XB4:
                    break;
                case 0XB5:
                    break;
                case 0XB6:
                    break;
                case 0XB7:
                    break;
                case 0XB8:
                    break;
                case 0XB9:
                    break;
                case 0XBA:
                    break;
                case 0XBB:
                    break;
                case 0XBC:
                    break;
                case 0XBD:
                    break;
                case 0XBE:
                    break;
                case 0XBF:
                    break;
                case 0xC0:
                    break;
                case 0xC1:
                    break;
                case 0xC2:
                    break;
                case 0xC3:
                    break;
                case 0xC4:
                    break;
                case 0xC5:
                    break;
                case 0xC6:
                    break;
                case 0xC7:
                    break;
                case 0xC8:
                    break;
                case 0xC9:
                    break;
                case 0xCA:
                    break;
                case 0xCB:
                    break;
                case 0xCC:
                    break;
                case 0xCD:
                    break;
                case 0xCE:
                    break;
                case 0xCF:
                    break;
                case 0xD0:
                    break;
                case 0xD1:
                    break;
                case 0xD2:
                    break;
                case 0xD3:
                    break;
                case 0xD4:
                    break;
                case 0xD5:
                    break;
                case 0xD6:
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
                    break;
                case 0xDC:
                    break;
                case 0xDD:
                    break;
                case 0xDE:
                    break;
                case 0xDF:
                    break;
                case 0xE0:
                    break;
                case 0xE1:
                    break;
                case 0xE2:
                    break;
                case 0xE3:
                    break;
                case 0xE4:
                    break;
                case 0xE5:
                    break;
                case 0xE6:
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
                    break;
                case 0xEE:
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
                    break;
                case 0xFE:
                    break;
                case 0xFF:
                    break;

            }

        }

        private void Handle_ED()
        {

        }


        private void Handle_FD()
        {
            throw new NotImplementedException();
        }

        private void SET(byte reg, int bitnumber)
        {
            reg |= (byte)(1 << bitnumber);
        }

        private void RES(byte reg, int bitnumber)
        {
            byte zero = 0xFE;
            zero = RL(zero, bitnumber);
            reg &= zero;
        }


        private void BIT(byte val, int bitNumber)
        {
            SetFlag(Flags.Z, (val & (1 << bitNumber)) == 0);
        }

        private byte RR(byte val, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                var temp = (byte)(val & (byte)1);
                val >>= 1;
                val |= temp;
            }
            return val;
        }

        private byte RL(byte val, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                var temp = (byte)(val & (byte)0x80); 
                val <<= 1;
                val |= temp;
            }
            return val;
        }

        private byte RRC(byte val, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                SetFlag(Flags.CF, (val & 0x80) != 0);
                val >>= 1;
                val |= CheckFlag(Flags.CF) ? (byte)0x80 : (byte)0;
            }

            return val;
        }

        private byte RLC(byte val, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                SetFlag(Flags.CF, (val & 0x80) != 0);
                val <<= 1;
                val |= CheckFlag(Flags.CF) ? (byte) 1 : (byte) 0;
            }

            return val;
        }

        private byte SLA(byte val , int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                SetFlag(Flags.CF, (val & 0x80) == 1);
                val <<= 1;
            }
            return val;
        }

        private byte SRA(byte val, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                SetFlag(Flags.CF, (val & 0x01) == 1);
                val >>= 1;
                val &= (byte)0x80;
            }
            return val;
        }

        private byte SLL(byte val, int amount)
        {
            return val;
        }

        private byte SRL(byte val, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                SetFlag(Flags.CF, (val & 0x01) == 1);
                val >>= 1;
            }
            return val;
        }


        private void DAA()
        {
            throw new NotImplementedException();
        }

        private void HALT()
        {
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

        private ushort FetchNextWord()
        {
            ushort val = (ushort)((Memory[PC] << 8) + Memory[++PC]);
            PC++;
            return val;
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