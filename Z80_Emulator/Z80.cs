﻿using System;
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
                case 0xCB: Handle_CB();
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
                case 0x40: BIT0(B);
                    break;
                case 0x41: BIT0(C);
                    break;
                case 0x42: BIT0(D);
                    break;
                case 0x43: BIT0(E);
                    break;
                case 0x44: BIT0(H);
                    break;
                case 0x45: BIT0(L);
                    break;
                case 0x46: BIT0(Memory[HL]);
                    break;
                case 0x47: BIT0(A);
                    break;
                case 0x48: BIT1(B);
                    break;
                case 0x49: BIT1(C);
                    break;
                case 0x4A: BIT1(D);
                    break;
                case 0x4B: BIT1(E);
                    break;
                case 0x4C: BIT1(H);
                    break;
                case 0x4D: BIT1(L);
                    break;
                case 0x4E: BIT1(Memory[HL]);
                    break;
                case 0x4F: BIT1(A);
                    break;
                case 0x50: BIT2(B);
                    break;
                case 0x51: BIT2(C);
                    break;
                case 0x52: BIT2(D);
                    break;
                case 0x53: BIT2(E);
                    break;
                case 0x54: BIT2(H);
                    break;
                case 0x55: BIT2(L);
                    break;
                case 0x56: BIT2(Memory[HL]);
                    break;
                case 0x57: BIT2(A);
                    break;
                case 0x58: BIT3(B);
                    break;
                case 0x59: BIT3(C);
                    break;
                case 0x5A: BIT3(D);
                    break;
                case 0x5B: BIT3(E);
                    break;
                case 0x5C: BIT3(H);
                    break;
                case 0x5D: BIT3(L);
                    break;
                case 0x5E: BIT3(Memory[HL]);
                    break;
                case 0x5F: BIT3(A);
                    break;
                case 0x60: BIT4(B);
                    break;
                case 0x61: BIT4(C);
                    break;
                case 0x62: BIT4(D);
                    break;
                case 0x63: BIT4(E);
                    break;
                case 0x64: BIT4(H);
                    break;
                case 0x65: BIT4(L);
                    break;
                case 0x66: BIT4(Memory[HL]);
                    break;
                case 0x67: BIT4(A);
                    break;
                case 0x68: BIT5(B);
                    break;
                case 0x69: BIT5(C);
                    break;
                case 0x6A: BIT5(D);
                    break;
                case 0x6B: BIT5(E);
                    break;
                case 0x6C: BIT5(H);
                    break;
                case 0x6D: BIT5(L);
                    break;
                case 0x6E: BIT5(Memory[HL]);
                    break;
                case 0x6F: BIT5(A);
                    break;
                case 0x70: BIT6(B);
                    break;
                case 0x71: BIT6(C);
                    break;
                case 0x72: BIT6(D);
                    break;
                case 0x73: BIT6(E);
                    break;
                case 0x74: BIT6(H);
                    break;
                case 0x75: BIT6(L);
                    break;
                case 0x76: BIT6(Memory[HL]);
                    break;
                case 0x77: BIT6(A);
                    break;
                case 0x78: BIT7(B);
                    break;
                case 0x79: BIT7(C);
                    break;
                case 0x7A: BIT7(D);
                    break;
                case 0x7B: BIT7(E);
                    break;
                case 0x7C: BIT7(H);
                    break;
                case 0x7D: BIT7(L);
                    break;
                case 0x7E: BIT7(Memory[HL]);
                    break;
                case 0x7F: BIT7(A);
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
        #region bit b,r  // will do logical OR with selected bit , if 0 then keep the Z flag on 1
        private void BIT7(byte val)
        {
            SetFlag(Flags.Z, (val |= 0x80) == 0); // 1000 0000 MSB 
        }

        private void BIT6(byte val)
        {
            SetFlag(Flags.Z, (val |= 0x40) == 0); // 0100 0000
        }

        private void BIT5(byte val)
        {
            SetFlag(Flags.Z, (val |= 0x20) == 0); // 0010 0000
        }

        private void BIT4(byte val)
        {
            SetFlag(Flags.Z, (val |= 0x10) == 0); // 0001 0000
        }

        private void BIT3(byte val)
        {
            SetFlag(Flags.Z, (val |= 0x08) == 0); // 0000 1000
        }

        private void BIT2(byte val)
        {
            SetFlag(Flags.Z, (val |= 0x04) == 0); // 0000 0100
        }

        private void BIT1(byte val)
        {
            SetFlag(Flags.Z, (val |= 0x02) == 0); // 0000 0010
        }

        private void BIT0(byte val)
        {
            SetFlag(Flags.Z, (val |= 0x01) == 0); // LSB 0000 0001
        }
        #endregion

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