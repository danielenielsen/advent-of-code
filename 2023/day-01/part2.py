
with open('input.txt', 'r') as file:
    input_text = file.read()

split_input = input_text.split('\n')
ready_list = list(filter(lambda x: x != '', split_input))

class CalibrationNumber:
    def __init__(self, index, number) -> None:
        self.index = index
        self.number = number
    
    def __str__(self) -> str:
        return f'({self.index}, {self.number})'

def get_numbers(string):
    string = string.lower()
    string_length = len(string)

    calibration_numbers = []

    for i in range(len(string)):
        if string[i].isdigit():
            calibration_numbers.append(CalibrationNumber(i, int(string[i])))
            continue
        
        if string_length - i + 1 >= 3 and string[i:i + 3] == 'one':
            calibration_numbers.append(CalibrationNumber(i, 1))
            continue
        
        if string_length - i + 1 >= 3 and string[i:i + 3] == 'two':
            calibration_numbers.append(CalibrationNumber(i, 2))
            continue
        
        if string_length - i + 1 >= 5 and string[i:i + 5] == 'three':
            calibration_numbers.append(CalibrationNumber(i, 3))
            continue
        
        if string_length - i + 1 >= 4 and string[i:i + 4] == 'four':
            calibration_numbers.append(CalibrationNumber(i, 4))
            continue
        
        if string_length - i + 1 >= 4 and string[i:i + 4] == 'five':
            calibration_numbers.append(CalibrationNumber(i, 5))
            continue
        
        if string_length - i + 1 >= 3 and string[i:i + 3] == 'six':
            calibration_numbers.append(CalibrationNumber(i, 6))
            continue

        if string_length - i + 1 >= 5 and string[i:i + 5] == 'seven':
            calibration_numbers.append(CalibrationNumber(i, 7))
            continue
        
        if string_length - i + 1 >= 5 and string[i:i + 5] == 'eight':
            calibration_numbers.append(CalibrationNumber(i, 8))
            continue
        
        if string_length - i + 1 >= 4 and string[i:i + 4] == 'nine':
            calibration_numbers.append(CalibrationNumber(i, 9))
            continue
    
    return calibration_numbers
    


def get_calibration_number(string):
    calibration_numbers = get_numbers(string)
    calibration_numbers.sort(key=lambda x: x.index)

    res = int(f'{calibration_numbers[0].number}{calibration_numbers[-1].number}')

    print(string)
    print(res)

    return res


res = sum(map(get_calibration_number, ready_list))
print(res)
