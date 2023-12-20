
with open('input.txt', 'r') as file:
    input_text = file.read()

split_input = input_text.split('\n')
ready_list = list(filter(lambda x: x != '', split_input))

def get_calibration_number(str):
    str_list = list(str)
    for i in range(len(str_list)):
        if str_list[i].isdigit():
            left_most = str_list[i]
            break
    
    str_list.reverse()
    for i in range(len(str_list)):
        if str_list[i].isdigit():
            right_most = str_list[i]
            break
    
    print(str)
    print(f'{left_most}, {right_most}')

    num = int(f'{left_most}{right_most}')
    print(num)

    return num

res = sum(list(map(get_calibration_number, ready_list)))
print(res)
