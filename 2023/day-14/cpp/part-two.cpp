#include <cstdio>
#include <cstring>
#include <iostream>
#include <fstream>
#include <string>
#include <unordered_map>

void move_specific_rock_north(int width, int height, char* dish, int index) {
    int free_index = index;
    while (true) {
        if (free_index < width) {
            break;
        }

        int new_index = free_index - width;
        if (dish[new_index] != 0) {
            break;
        }

        free_index -= width;
    }

    dish[index] = 0;
    dish[free_index] = 2;
}

void move_specific_rock_south(int width, int height, char* dish, int index) {
    int free_index = index;
    while (true) {
        if (free_index >= (height - 1) * width) {
            break;
        }

        int new_index = free_index + width;
        if (dish[new_index] != 0) {
            break;
        }

        free_index += width;
    }

    dish[index] = 0;
    dish[free_index] = 2;
}

void move_specific_rock_west(int width, int height, char* dish, int index) {
    int free_index = index;
    while (true) {
        if (free_index % width == 0) {
            break;
        }

        int new_index = free_index - 1;
        if (dish[new_index] != 0) {
            break;
        }

        free_index--;
    }

    dish[index] = 0;
    dish[free_index] = 2;
}

void move_specific_rock_east(int width, int height, char* dish, int index) {
    int free_index = index;
    while (true) {
        if (free_index % width == width - 1) {
            break;
        }

        int new_index = free_index + 1;
        if (dish[new_index] != 0) {
            break;
        }

        free_index++;
    }

    dish[index] = 0;
    dish[free_index] = 2;
}

void tilt_north(int width, int height, char* dish) {
    for (int i = 0; i < height; i++) {
        for (int j = 0; j < width; j++) {
            int index = i * height + j;
            if (dish[index] == 2) {
                move_specific_rock_north(width, height, dish, index);
            }
        }
    }
}

void tilt_south(int width, int height, char* dish) {
    for (int i = height - 1; i >= 0; i--) {
        for (int j = 0; j < width; j++) {
            int index = i * height + j;
            if (dish[index] == 2) {
                move_specific_rock_south(width, height, dish, index);
            }
        }
    }
}

void tilt_west(int width, int height, char* dish) {
    for (int i = 0; i < height; i++) {
        for (int j = 0; j < width; j++) {
            int index = i * height + j;
            if (dish[index] == 2) {
                move_specific_rock_west(width, height, dish, index);
            }
        }
    }
}

void tilt_east(int width, int height, char* dish) {
    for (int i = 0; i < height; i++) {
        for (int j = width - 1; j >= 0; j--) {
            int index = i * height + j;
            if (dish[index] == 2) {
                move_specific_rock_east(width, height, dish, index);
            }
        }
    }
}

void tilt_round(int width, int height, char* dish) {
    tilt_north(width, height, dish);
    tilt_west(width, height, dish);
    tilt_south(width, height, dish);
    tilt_east(width, height, dish);
}

int calculate_load(int width, int height, char* dish) {
    int load = 0;

    for (int i = 0; i < height; i++) {
        for (int j = 0; j < width; j++) {
            int index = i * height + j;
            
            if (dish[index] == 2) {
                load += height - i;
            }
        }
    }

    return load;
}

void print_dish(int width, int height, char* dish) {
    for (int i = 0; i < height; i++) {
        for (int j = 0; j < width; j++) {
            int index = i * height + j;

            if (dish[index] == 0) {
                std::cout << ".";
            } else if (dish[index] == 1) {
                std::cout << "#";
            } else {
                std::cout << "O";
            }
        }

        std::cout << std::endl;
    }
}

std::string get_dish_string(int width, int height, char* dish) {
    std::string dish_string;
    dish_string.reserve(width * height);

    for (int i = 0; i < height; i++) {
        for (int j = 0; j < width; j++) {
            int index = i * height + j;

            if (dish[index] == 0) {
                dish_string.push_back('.');
            } else if (dish[index] == 1) {
                dish_string.push_back('#');
            } else if (dish[index] == 2) {
                dish_string.push_back('O');
            }
        }
    }

    return dish_string;
}

int main() {
    std::ifstream file("../input.txt");

    if (!file.is_open()) {
        std::cerr << "Could not open input file" << std::endl;
        return 1;
    }

    std::string full_file;
    int width = 0;
    int height = 0;

    std::string line;
    while (getline(file, line)) {
        width = line.length();
        height += 1;
        full_file += line;
    }
    file.close();

    char dish[width * height];
    std::memset(dish, 0, width * height);

    for (int i = 0; i < height; i++) {
        for (int j = 0; j < width; j++) {
            int index = i * height + j;
            
            if (full_file[index] == '#') {
                dish[index] = 1;
            } else if (full_file[index] == 'O') {
                dish[index] = 2;
            }
        }
    }

    std::unordered_map<std::string, int> hashmap;

    for (int i = 0; i < 1000000000; i++) {
        std::string str = get_dish_string(width, height, dish);
        if (hashmap.find(str) != hashmap.end()) {
            int j = hashmap[str];

            int cycle_length = i - j;
            int cycles_to_skip = (1000000000 - i) / cycle_length;
            i += cycles_to_skip * cycle_length;
        }

        hashmap[str] = i;
        tilt_round(width, height, dish);
    }

    int load = calculate_load(width, height, dish);
    std::cout << "Load: " << load << std::endl;
}
