namespace Part1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../../input.txt";
            string input = File.ReadAllText(path).Replace("\r", "").Trim();
            CPU cpu = CPU.ParseInput(input);
            List<int> output = cpu.RunProgram();

            Console.WriteLine($"Result: {string.Join(",", output)}");
        }
    }

    class CPU
    {
        private int _registerA;
        private int _registerB;
        private int _registerC;

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

        public List<int> RunProgram()
        {
            List<int> output = new List<int>();

            while (_instructionPointer < _instructions.Count)
            {
                int opcode = _instructions[_instructionPointer];
                Instruction instruction = GetInstructionFromOpCode(opcode);
                int operand = _instructions[_instructionPointer + 1];

                output.AddRange(HandleInstruction(instruction, operand));
            }

            return output;
        }

        private List<int> HandleInstruction(Instruction instruction, int operand)
        {
            if (operand < 0 || operand > 7)
            {
                throw new Exception($"Invalid operand. Expected 0-7 but got '{operand}'.");
            }

            List<int> output = new List<int>();

            switch (instruction)
            {
                case Instruction.ADV:
                    int numerator1 = _registerA;
                    int denominator1 = (int)Math.Pow(2, HandleCombo(operand));

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
                        _instructionPointer = operand;
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
                    output.Add(HandleCombo(operand) % 8);
                    _instructionPointer += 2;
                    break;
                case Instruction.BDV:
                    int numerator2 = _registerA;
                    int denominator2 = (int)Math.Pow(2, HandleCombo(operand));

                    if (denominator2 == 0)
                    {
                        throw new Exception("Cannot divide by zero.");
                    }

                    _registerB = numerator2 / denominator2;
                    _instructionPointer += 2;
                    break;
                case Instruction.CDV:
                    int numerator3 = _registerA;
                    int denominator3 = (int)Math.Pow(2, HandleCombo(operand));

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

        private int HandleCombo(int combo)
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

        private static Instruction GetInstructionFromOpCode(int opcode)
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
