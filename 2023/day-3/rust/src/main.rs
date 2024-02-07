use std::fs::File;
use std::io::Read;

use std::collections::HashMap;

fn main() -> Result<(), String>  {
    let file_name = "../input.txt";
    let file_contents = read_input_file(file_name)?;
    let matrix = parse_to_matrix(&file_contents);

    let part_one_res = part_one(&matrix);
    let part_two_res = part_two(&matrix);

    println!("Part 1 res {}", part_one_res);
    println!("Part 2 res {}", part_two_res);

    Ok(())
}

fn part_one(matrix: &Matrix) -> i32 {
    let numbers: Vec<MatrixNumber> = get_numbers_from_matrix(matrix);

    let valid_numbers: Vec<&MatrixNumber> = numbers.iter().filter(|num| {
        let surrounding_cells: Vec<(usize, usize)> = num.positions.iter().flat_map(|(i, j)| {
            get_surrounding_positions((*i, *j), matrix.width, matrix.height)
        }).collect();

        surrounding_cells.iter().any(|(i,  j)| {
            let cell = &matrix.cells[*i][*j];

            if cell.character == '.' {
                return false;
            }

            if cell.character.is_digit(10) {
                return false;
            }

            true
        })
    }).collect();

    valid_numbers.iter().map(|x| x.num).sum()
}

fn part_two(matrix: &Matrix) -> i32 {
    let numbers: Vec<MatrixNumber> = get_numbers_from_matrix(matrix);
    let pos_num_pair: Vec<_> = numbers.iter().flat_map(|num| {
        num.positions.iter().map(move |pos| (pos, num))
    }).collect();
    let pos_to_num_map: HashMap<&(usize, usize), &MatrixNumber> = HashMap::from_iter(pos_num_pair);
    let asterisk_cells: Vec<_> = matrix.cells.iter().flat_map(|cells| cells).filter(|cell| cell.character == '*').collect();

    let mut res = 0;
    for cell in asterisk_cells {
        let surrounding_positions = get_surrounding_positions((cell.column, cell.row), matrix.width, matrix.height);
        let mut surrounding_numbers = Vec::new();

        for pos in surrounding_positions {
            let num = match pos_to_num_map.get(&pos) {
                None => continue,
                Some(&val) => val
            };

            if !surrounding_numbers.contains(&num) {
                surrounding_numbers.push(num);
            }
        }

        if surrounding_numbers.iter().count() != 2 {
            continue;
        }

        res += surrounding_numbers.iter().map(|item| item.num).fold(1, |acc, val| acc * val);
    }

    res
}

fn get_surrounding_positions(pos: (usize, usize), width: usize, height: usize) -> Vec<(usize, usize)> {
    let (pos_width, pos_height) = pos;
    let offsets = [(0, 1), (1, 1), (1, 0), (1, -1), (0, -1), (-1, -1), (-1, 0), (-1, 1)];

    let mut surroundings = Vec::new();

    for (width_offset, height_offset) in offsets {
        let new_pos_width = (pos_width as i32) + width_offset;
        let new_pos_height = (pos_height as i32) + height_offset;

        if new_pos_width < 0 || new_pos_width >= width as i32 || new_pos_height < 0 || new_pos_height >= height as i32 {
            continue;
        }

        surroundings.push((new_pos_width as usize, new_pos_height as usize));
    }

    surroundings
}

fn get_numbers_from_matrix(matrix: &Matrix) -> Vec<MatrixNumber> {
    let mut numbers: Vec<MatrixNumber> = Vec::new();

    for j in 0..matrix.height {
        let mut num_str = "".to_string();
        let mut num_positions = Vec::new();

        for i in 0..matrix.width {
            let cell_char = matrix.cells[i][j].character;

            if !cell_char.is_digit(10) {
                if num_str.len() > 0 {
                    numbers.push(MatrixNumber {
                        num: num_str.parse().unwrap(),
                        positions: num_positions.clone()
                    })
                }

                num_str = "".to_string();
                num_positions = Vec::new();

                continue;
            }

            num_str.push(cell_char);
            num_positions.push((i, j));
        }

        if num_str.len() > 0 {
            numbers.push(MatrixNumber {
                num: num_str.parse().unwrap(),
                positions: num_positions.clone()
            })
        }

        num_str = "".to_string();
        num_positions = Vec::new();
    }

    numbers
}

fn parse_to_matrix(input: &str) -> Matrix {
    let lines: Vec<_> = input.lines().filter(|c| !c.is_empty()).collect();

    let height = lines.iter().count();
    let width = lines.first().map_or(0, |s| s.len());

    let mut cells: Vec<Vec<_>> = (0..width).map(|_| Vec::with_capacity(height)).collect();

    for (height_idx, line) in lines.iter().enumerate() {
        for (width_idx, chr) in line.chars().enumerate() {
            let cell = Cell {
                column: width_idx,
                row: height_idx,
                character: chr
            };

            cells[width_idx].push(cell);
        }
    }

    Matrix {
        cells,
        width,
        height
    }
}

fn read_input_file(file_name: &str) -> Result<String, String> {
    let mut file = match File::open(file_name) {
        Ok(f) => f,
        Err(_) => {
            return Err(format!("FAILED: The file \"{}\" could not be opened", file_name));
        }
    };

    let mut contents = String::new();
    let bytes_read = match file.read_to_string(&mut contents) {
        Ok(res) => res,
        Err(_) => {
            return Err(format!("FAILED: The file \"{}\" could not be read", file_name));
        }
    };

    println!("{} bytes were read from \"{}\"", bytes_read, file_name);

    Ok(contents)
}

#[derive(Debug)]
struct MatrixNumber {
    pub num: i32,
    pub positions: Vec<(usize, usize)>
}

impl PartialEq for MatrixNumber {
    fn eq(&self, other: &Self) -> bool {
        if self.num != other.num {
            return false;
        }

        if !self.positions.eq(&other.positions) {
            return false;
        }

        true
    }
}

#[derive(Debug)]
struct Cell {
    pub row: usize,
    pub column: usize,
    pub character: char
}

#[derive(Debug)]
struct Matrix {
    pub width: usize,
    pub height: usize,
    pub cells: Vec<Vec<Cell>>
}

impl Matrix {
    pub fn print(&self) {
        for j in 0..self.height {
            for i in 0..self.width {
                print!("{}", self.cells[i][j].character);
            }
            println!();
        }
    }
}
