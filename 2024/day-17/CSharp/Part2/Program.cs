namespace Part2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../../input.txt";
            string input = File.ReadAllText(path).Replace("\r", "").Trim();
            CPU cpu = CPU.ParseInput(input);
            long smallestA = cpu.FindSmallestA();

            Console.WriteLine($"Smallest A: {smallestA}");
        }
    }

    class CPU
    {
        private long _registerA;
        private long _registerB;
        private long _registerC;

        private int _instructionPointer;
        private List<int> _instructions;

        private CPU(int registerA, int registerB, int registerC, List<int> instructions)
        {
            _registerA = registerA;
            _registerB = registerB;
            _registerC = registerC;

            _instructionPointer = 0;
            _instructions = instructions;
        }

        public static CPU ParseInput(string input)
        {
            string[] parts = input.Split("\n\n");

            string[] registers = parts[0].Split("\n");
            int registerA = int.Parse(registers[0].Split(": ")[1]);
            int registerB = int.Parse(registers[1].Split(": ")[1]);
            int registerC = int.Parse(registers[2].Split(": ")[1]);

            List<int> instructions = parts[1].Split(": ")[1].Split(",").Select(int.Parse).ToList();

            return new CPU(registerA, registerB, registerC, instructions);
        }

        public long FindSmallestA()
        {
            long a = 0;
            while (true)
            {
                bool valid = CheckA(a);

                if (valid)
                {
                    return a;
                }

                a += 1;
            }
        }

        private bool CheckA(long a)
        {
            _registerA = a;
            _registerB = 0;
            _registerC = 0;
            _instructionPointer = 0;

            List<int> output = new List<int>();

            while (_instructionPointer < _instructions.Count)
            {
                int opcode = _instructions[_instructionPointer];
                Instruction instruction = GetInstructionFromOpCode(opcode);
                int operand = _instructions[_instructionPointer + 1];

                int? newOutput = HandleInstruction(instruction, operand);

                if (!newOutput.HasValue)
                {
                    continue;
                }

                if (newOutput.Value != _instructions[output.Count])
                {
                    return false;
                }

                output.Add(newOutput.Value);
            }

            if (output.SequenceEqual(_instructions))
            {
                return true;
            }

            return false;
        }

        private int? HandleInstruction(Instruction instruction, int operand)
        {
            if (operand < 0 || operand > 7)
            {
                throw new Exception($"Invalid operand. Expected 0-7 but got '{operand}'.");
            }

            int? output = null;

            switch (instruction)
            {
                case Instruction.ADV:
                    long numerator1 = _registerA;
                    long denominator1 = (long)Math.Pow(2, HandleCombo(operand));

                    if (denominator1 == 0)
                    {
                        throw new Exception("Cannot divide by zero.");
                    }

                    _registerA = numerator1 / denominator1;
                    _instructionPointer += 2;
                    break;
                case Instruction.BXL:
                    _registerB = _registerB ^ operand;
                    _instructionPointer += 2;
                    break;
                case Instruction.BST:
                    _registerB = HandleCombo(operand) % 8;
                    _instructionPointer += 2;
                    break;
                case Instruction.JNZ:
                    if (_registerA != 0)
                    {
                        _instructionPointer = (int)operand;
                    }
                    else
                    {
                        _instructionPointer += 2;
                    }
                    break;
                case Instruction.BXC:
                    _registerB = _registerB ^ _registerC;
                    _instructionPointer += 2;
                    break;
                case Instruction.OUT:
                    output = (int)(HandleCombo(operand) % 8);
                    _instructionPointer += 2;
                    break;
                case Instruction.BDV:
                    long numerator2 = _registerA;
                    long denominator2 = (long)Math.Pow(2, HandleCombo(operand));

                    if (denominator2 == 0)
                    {
                        throw new Exception("Cannot divide by zero.");
                    }

                    _registerB = numerator2 / denominator2;
                    _instructionPointer += 2;
                    break;
                case Instruction.CDV:
                    long numerator3 = _registerA;
                    long denominator3 = (long)Math.Pow(2, HandleCombo(operand));

                    if (denominator3 == 0)
                    {
                        throw new Exception("Cannot divide by zero.");
                    }

                    _registerC = numerator3 / denominator3;
                    _instructionPointer += 2;
                    break;
                default:
                    throw new Exception($"Cannot handle unknown instruction '{instruction}'.");
            }

            return output;
        }

        private long HandleCombo(long combo)
        {
            return combo switch
            {
                0 or 1 or 2 or 3 => combo,
                4 => _registerA,
                5 => _registerB,
                6 => _registerC,
                _ => throw new Exception($"Invalid combo. Expected 0-6 but got '{combo}'.")
            };
        }

        private static Instruction GetInstructionFromOpCode(long opcode)
        {
            return opcode switch
            {
                0 => Instruction.ADV,
                1 => Instruction.BXL,
                2 => Instruction.BST,
                3 => Instruction.JNZ,
                4 => Instruction.BXC,
                5 => Instruction.OUT,
                6 => Instruction.BDV,
                7 => Instruction.CDV,
                _ => throw new Exception($"Invalid opcode. Expected 0-7 but got '{opcode}'.")
            };
        }

    }

    enum Instruction
    {
        ADV,
        BXL,
        BST,
        JNZ,
        BXC,
        OUT,
        BDV,
        CDV,
    }
}
