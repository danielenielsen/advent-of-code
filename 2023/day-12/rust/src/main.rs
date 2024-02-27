use std::usize;
use std::fs::File;
use std::io::Read;

use std::collections::HashMap;

use nom::{
    IResult,
    Parser,
    multi::{separated_list1, many0},
    combinator::map_res,
    character::complete::{one_of, multispace1, digit1},
    bytes::complete::tag,
};

#[derive(Debug, Clone, Copy, PartialEq, Eq)]
enum SpringType {
    Unknown,
    Damaged,
    Operational
}

impl SpringType {
    fn from_char(chr: char) -> Result<SpringType, String> {
        match chr {
            '?' => Ok(SpringType::Unknown),
            '#' => Ok(SpringType::Damaged),
            '.' => Ok(SpringType::Operational),
            _ => Err(format!("the given char {} could not be converted to a spring", chr))
        }
    }
}

#[derive(Debug)]
struct Spring {
    springs: Vec<SpringType>,
    records: Vec<i64>,
}

fn main() -> Result<(), String> {
    let start = std::time::Instant::now();
    let file_name = "../input.txt";
    let file_contents = read_input_file(file_name)?;
    let springs = parse_input(&file_contents)?;
    println!("Reading and parsing done in {:#?}", start.elapsed());
    println!();

    let part_one_start = std::time::Instant::now();
    let part_one_res = part_one(&springs)?;
    let part_one_time = part_one_start.elapsed();

    let part_two_start = std::time::Instant::now();
    let part_two_res = part_two(&springs)?;
    let part_two_time = part_two_start.elapsed();

    println!("Part 1 res: {} ({:#?})", part_one_res, part_one_time);
    println!("Part 2 res: {} ({:#?})", part_two_res, part_two_time);

    Ok(())
}

fn part_one(springs: &[Spring]) -> Result<i64, String> {
    let res: i64 = springs.iter().map(|spring| find_spring_combinations(spring)).sum();
    Ok(res)
}

fn part_two(springs: &[Spring]) -> Result<i64, String> {
    let springs: Vec<_> = springs.iter().map(|spring| Spring {
        springs: repeat_with_delimiter(&spring.springs, &SpringType::Unknown, 5),
        records: spring.records.repeat(5)
    }).collect();

    let res: i64 = springs.iter().map(|spring| find_spring_combinations(spring)).sum();
    Ok(res)
}

fn brute_force(spring: &Spring) -> i64 {

    fn map_springs_to_bits(spring: &Spring, spring_type: SpringType) -> u128 {
        let mut res = 0u128;

        for (idx, spring) in spring.springs.iter().enumerate() {
            if *spring == spring_type {
                res += 1 << idx;
            }
        }

        res
    }

    fn setup_start_bits(spring: &Spring) -> u128 {
        let mut res = 0u128;
        let mut current_pos = 0;

        for consecutive in &spring.records {
            for _ in 0..*consecutive {
                res += 1 << current_pos;
                current_pos += 1;
            }

            current_pos += 1;
        }
        
        res
    }
    
    fn is_valid_placement(damaged_spring_bits: u128, operational_spring_bits: u128, current: u128) -> bool {
        if damaged_spring_bits & current != damaged_spring_bits {
            return false;
        }

        if operational_spring_bits & current != 0 {
            return false;
        }

        true
    }

    fn make_mask_hashmap() -> HashMap<usize, u128> {
        let mut hashmap: HashMap<usize, u128> = HashMap::new();

        for i in 0..128 {
            let mut res = 0;

            for j in 0..=i {
                res += 1 << j;
            }

            hashmap.insert(i, res);
        }

        hashmap
    }

    fn move_to_next_position(spring: &Spring, hashmap: &HashMap<usize, u128>, current: &mut u128) -> bool {
        let mut current_pos = spring.springs.len() - 1;

        for (idx, record) in spring.records.iter().enumerate().rev() {
            if *current & (1 << current_pos) == 0 {
                *current &= hashmap.get(&current_pos).unwrap();
                let leading = current.leading_zeros();
                current_pos = 128 - leading as usize;
                *current += 1 << current_pos;
                *current -= 1 << (current_pos - *record as usize);
                
                current_pos += 2;
                for i in (idx + 1)..spring.records.len() {
                    for _ in 0..spring.records[i] {
                        *current += 1 << current_pos;
                        current_pos += 1;
                    }

                    current_pos += 1;
                }
                
                return true;
            }

            if *record as usize + 1 <= current_pos {
                current_pos -= *record as usize + 1;
            }
        }

        false
    }

    let damaged_spring_bits = map_springs_to_bits(&spring, SpringType::Damaged);
    let operational_spring_bits = map_springs_to_bits(&spring, SpringType::Operational);
    let hashmap = make_mask_hashmap();

    let mut count: i64 = 0;
    let mut current = setup_start_bits(&spring);

    loop {
        if is_valid_placement(damaged_spring_bits, operational_spring_bits, current) {
            println!("Correct: {current:b}");
            count += 1;
        }

        if !move_to_next_position(&spring, &hashmap, &mut current) {
            break;
        }
    }

    count
}

fn find_spring_combinations(spring: &Spring) -> i64 { 
    fn make_string_from_groups(offset: i64, group_sizes: &[i64], group_positions: &[i64]) -> String {
        let mut str = "".to_string();

        for i in 0..group_sizes.len() {
            let size = group_sizes[i];
            let position = group_positions[i] + offset;

            str.push_str(format!("{size};{position},").as_str());
        }

        str
    }
    
    fn check_validity(springs: &[SpringType], group_sizes: &[i64], group_positions: &[i64]) -> bool {
        assert_eq!(group_sizes.len(), group_positions.len());

        let mut damaged_positions_covered: Vec<usize> = springs.iter().enumerate().filter(|(_, &x)| x == SpringType::Damaged).map(|(idx, _)| idx).collect();
        
        for i in 0..group_sizes.len() {
            let position = group_positions[i];
            let size = group_sizes[i];
            
            for j in 0..size {
                let cursor = (position + j) as usize;
                if springs[cursor] == SpringType::Operational {
                    return false;
                }

                if damaged_positions_covered.contains(&cursor) {
                    let idx = damaged_positions_covered.iter().position(|x| *x == cursor).unwrap();
                    damaged_positions_covered.swap_remove(idx);
                }
            }
        }

        if damaged_positions_covered.len() > 0 {
            return false;
        }

        true
    }

    fn recursive_check(springs: &[SpringType], offset: i64, group_sizes: &[i64], group_positions: &[i64], solutions_map: &mut HashMap<String, i64>) -> i64 {
        assert_eq!(group_sizes.len(), group_positions.len());
        assert!(offset >= 0);

        if springs[0] == SpringType::Damaged && group_positions[0] > 0 {
            return 0;
        }

        let hash_str = make_string_from_groups(offset, &group_sizes, &group_positions);

        if solutions_map.contains_key(&hash_str) {
            let res = *solutions_map.get(&hash_str).unwrap();
            return res;
        }

        let mut sum = 0;

        if group_sizes.len() > 1 {
            let spring_separator = (group_positions.get(0).unwrap() + group_sizes.get(0).unwrap() + 1) as usize;
            let first_part = &springs[..spring_separator];
            let second_part = &springs[spring_separator..];

            if check_validity(&first_part, &group_sizes[..1], &group_positions[..1]) {
                let new_positions: Vec<_> = group_positions[1..].iter().map(|x| x - spring_separator as i64).collect();
                let sub_problem_res = recursive_check(second_part, offset + spring_separator as i64, &group_sizes[1..], &new_positions, solutions_map);
                sum += sub_problem_res;
            }
        }

        if group_sizes.len() == 1 && group_positions[0] + group_sizes[0] == springs.len() as i64 {
            if check_validity(springs, group_sizes, group_positions) {
                return 1;
            }
        }

        let incremented_positions: Vec<_> = group_positions.iter().map(|x| x + 1).collect();
        let last_position = incremented_positions[incremented_positions.len() - 1];
        let last_size = group_sizes[group_sizes.len() - 1];


        if last_position + last_size < springs.len() as i64 + 1 {
            let first_position = group_positions[0];
            let (new_springs, offset_delta) = if first_position == 0 {
                (springs, 0)
            } else {
                (&springs[1..], 1)
            };
            let new_positions = if offset_delta == 1 {
                group_positions
            } else {
                &incremented_positions
            };

            let sub_problem_res = recursive_check(new_springs, offset + offset_delta, group_sizes, new_positions, solutions_map);
            sum += sub_problem_res;

            if group_sizes.len() == 1 && check_validity(springs, group_sizes, group_positions) {
                sum += 1;
            }
        }

        solutions_map.insert(hash_str, sum);

        sum
    }


    let group_sizes: Vec<i64> = spring.records.clone();
    let mut group_positions: Vec<i64> = Vec::new();

    let mut current = 0;
    for item in &group_sizes {
        group_positions.push(current);
        current += item + 1;
    }

    recursive_check(&spring.springs, 0, &group_sizes, &group_positions, &mut HashMap::new())
}

fn repeat_with_delimiter<T: Clone>(vec: &[T], delimiter: &T, times: usize) -> Vec<T> {
    let mut res: Vec<T> = Vec::new();

    for i in 0..times {
        for item in vec {
            res.push(item.clone());
        }

        if i + 1 < times {
            res.push(delimiter.clone());
        }
    }

    res
}

fn parse_input(input: &str) -> Result<Vec<Spring>, String> {
    let lines: Vec<&str> = input.lines().filter(|x| !x.is_empty()).collect();

    let mut res = Vec::new();
    for line in lines {
        match parse_line(line) {
            Ok((_, val)) => res.push(val),
            Err(_) => return Err(format!("Could not parse the line \"{}\"", line))
        }
    }

    Ok(res)
}

fn parse_line(input: &str) -> IResult<&str, Spring> {
   let (input, springs) = many0(map_res(one_of("?#."), SpringType::from_char)).parse(input)?;
   let (input, _) = multispace1.parse(input)?;
   let (input, records) = separated_list1(tag(","), map_res(digit1, str::parse::<i64>)).parse(input)?;

   Ok((input, Spring {
       springs,
       records,
   }))
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

