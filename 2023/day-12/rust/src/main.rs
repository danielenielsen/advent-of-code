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
    let file_name = "../input2.txt";
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

    let mut res = 0;

    for spring in springs {
        res += brute_force(&spring);
    }

    Ok(res)
}

fn part_two(springs: &[Spring]) -> Result<i64, String> {
    Ok(0)
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
            count += 1;
        }

        if !move_to_next_position(&spring, &hashmap, &mut current) {
            break;
        }
    }

    count
}

fn solve(springs: &Spring) -> i64 {
    0
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

