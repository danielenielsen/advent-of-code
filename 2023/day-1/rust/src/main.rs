use std::collections::HashMap;
use std::fs::File;
use std::io::Read;


fn main() -> Result<(), String> {
    let file_name = "../input.txt";
    let file_contents = read_input_file(file_name)?;

    let num_sum_one: i32 = part_one(&file_contents)?;
    let num_sum_two: i32 = part_two(&file_contents)?;

    println!("The sum of the calibrations for part 1 is {}", num_sum_one);
    println!("The sum of the calibrations for part 2 is {}", num_sum_two);
    Ok(())
}

fn part_one(file_contents: &String) -> Result<i32, String> {
    let num_sum: i32 = file_contents.split('\n').try_fold(0, |acc: i32, s: &str | {
        let digits: Vec<char>= s.chars().filter(|c: &char| c.is_digit(10)).collect();

        let first_num: &char = digits.first().unwrap_or(&'0');
        let last_num: &char = digits.last().unwrap_or(&'0');
        let combined_num = format!("{}{}", first_num, last_num);

        let num: i32 = match combined_num.parse() {
            Ok(n) => n,
            Err(_) => {
                return Err(format!("Could not parse {} as an integer", combined_num));
            }
        };

        Ok(acc + num)
    })?;

    Ok(num_sum)
}

fn part_two(file_contents: &String) -> Result<i32, String> {
    let patterns = [("one", '1'), ("two", '2'), ("three", '3'), ("four", '4'), ("five", '5'), ("six", '6'), ("seven", '7'), ("eight", '8'), ("nine", '9')];

    let mut num_sum = 0;

    for s in file_contents.split('\n') {

        if s == "" {
            continue;
        }

        let mut hashmap: HashMap<usize, char> = HashMap::new();

        for (idx, c) in s.chars().enumerate() {
            if c.is_digit(10) {
                hashmap.insert(idx, c);
            }
        }

        for (pattern, val) in patterns {
            for (idx, _) in s.match_indices(pattern) {
                hashmap.insert(idx, val);
            }
        }

        let mut indexes: Vec<usize> = hashmap.keys().cloned().collect();
        indexes.sort();

        let first_num: &char = hashmap.get(indexes.first().unwrap()).unwrap_or(&'0');
        let last_num: &char = hashmap.get(indexes.last().unwrap()).unwrap_or(&'0');
        let combined_num: String = format!("{}{}", first_num, last_num);

        let num: i32 = match combined_num.parse() {
            Ok(n) => n,
            Err(err) => return Err(format!("{}", err))
        };

        num_sum += num;
    }

    Ok(num_sum)
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

