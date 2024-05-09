#include <cstdio>
#include <cstring>
#include <iostream>
#include <fstream>

void move_specific_rock_north(int width, int height, char* dish, int index) {
    int free_index_above = index;
    while (true) {
        if (free_index_above < width) {
            break;
        }

        int index_above = free_index_above - width;
        if (dish[index_above] != 0) {
            break;
        }

        free_index_above -= width;
    }

    dish[index] = 0;
    dish[free_index_above] = 2;
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

    tilt_north(width, height, dish);
    int load = calculate_load(width, height, dish);

    std::cout << "Load: " << load << std::endl;
}
