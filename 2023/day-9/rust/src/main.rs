use std::fs::File;
use std::io::Read;

use nom::{
    character::complete::{char, digit1, multispace1},
    multi::{separated_list1},
    combinator::{map_res, opt},
    IResult,
    Parser
};

fn main() -> Result<(), String> {
    let start = std::time::Instant::now();
    let file_name = "../input.txt";
    let file_contents = read_input_file(file_name)?;
    let parse_information = parse_input(&file_contents)?;
    println!("Reading and parsing done in {:#?}", start.elapsed());
    println!();

    let part_one_start = std::time::Instant::now();
    let part_one_res = part_one(&parse_information)?;
    let part_one_time = part_one_start.elapsed();

    let part_two_start = std::time::Instant::now();
    let part_two_res = part_two(&parse_information)?;
    let part_two_time = part_two_start.elapsed();

    println!("Part 1 res: {} ({:#?})", part_one_res, part_one_time);
    println!("Part 2 res: {} ({:#?})", part_two_res, part_two_time);

    Ok(())
}

fn part_one(data: &Vec<Vec<i32>>) -> Result<i32, String> {
    let mut res = 0;
    for (idx, line) in data.iter().enumerate() {
        let differences = data_line_to_differences(line);
        let extrapolation = extrapolate_forward(&differences);

        res += extrapolation;
    }

    Ok(res)
}

fn part_two(data: &Vec<Vec<i32>>) -> Result<i32, String> {
    let mut res = 0;
    for (idx, line) in data.iter().enumerate() {
        let differences = data_line_to_differences(line);
        let extrapolation = extrapolate_backward(&differences);

        res += extrapolation;
    }

    Ok(res)
}

fn extrapolate_forward(data: &Vec<Vec<i32>>) -> i32 {
    return data.iter().map(|x| x.last().unwrap()).sum();
}

fn extrapolate_backward(data: &Vec<Vec<i32>>) -> i32 {
    return data.iter().map(|x| x.first().unwrap()).rev().fold(0, |acc, val| val - acc)
}

fn data_line_to_differences(data: &Vec<i32>) -> Vec<Vec<i32>> {
    let mut res = vec![data.clone()];

    while res.last().unwrap().iter().any(|&x| x != 0) {
        let prev = res.last().unwrap();
        let mut new = Vec::with_capacity(prev.len() - 1);

        for i in 0..prev.len() - 1 {
            new.push(prev[i + 1] - prev[i]);
        }

        res.push(new);
    }

    return res;
}

fn parse_input(input: &str) -> Result<Vec<Vec<i32>>, String> {
    let lines = input.lines().filter(|x| !x.is_empty()).collect::<Vec<_>>();

    let mut res = Vec::new();
    for line in lines {
        match parse_line(line) {
            Ok((_, val)) => res.push(val),
            Err(_) => return Err(format!("Could not parse the line \"{}\"", line))
        }
    }

    return Ok(res);
}

fn parse_line(input: &str) -> IResult<&str, Vec<i32>> {
    separated_list1(multispace1, parse_number).parse(input)
}

fn parse_number(input: &str) -> IResult<&str, i32> {
    let (input, sign) = opt(char('-')).parse(input)?;
    let (input, mut number) = map_res(digit1, str::parse::<i32>).parse(input)?;

    if let Some('-') = sign {
        number = -1 * number;
    }

    Ok((input, number))
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
