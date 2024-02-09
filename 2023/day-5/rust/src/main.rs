use std::fs::File;
use std::io::Read;

use nom::{
    bytes::complete::tag,
    character::complete::{digit1, multispace1},
    multi::separated_list1,
    combinator::map_res,
    IResult,
    Parser
};
use nom::character::complete::{alpha1, newline};


fn main() -> Result<(), String> {
    let start = std::time::Instant::now();
    let file_name = "../input.txt";
    let file_contents = read_input_file(file_name)?;

    let problem_input = match parse_input(&file_contents) {
        Ok((_, val)) => val,
        Err(_) => return Err("Parsing failed".to_string())
    };

    let part_one_res: i64 = part_one(&problem_input)?;
    let part_two_res: i64 = part_two(&problem_input)?;

    println!("Part 1 res: {}", part_one_res);
    println!("Part 2 res: {}", part_two_res);

    println!("Elapsed time: {:?}", start.elapsed());

    Ok(())
}

fn part_one(problem_input: &ProblemInput) -> Result<i64, String> {
    let min_value = problem_input.seeds.iter().map(|&num| {
        pass_through_all_mappings(num, &problem_input.mapping_sets)
    }).min();

    let min_value = match min_value {
        None => return Err("Could not get min value".to_string()),
        Some(val) => val
    };

    Ok(min_value)
}

fn part_two(problem_input: &ProblemInput) -> Result<i64, String> {
    let mut res = i64::MAX;

    for i in (0..problem_input.seeds.len()).step_by(2) {
        let start = problem_input.seeds[i];
        let length = problem_input.seeds[i + 1];

        for j in start..start + length {
            let num = pass_through_all_mappings(j, &problem_input.mapping_sets);
            res = i64::min(res, num);
        }
    }

    Ok(res)
}

fn pass_through_all_mappings(number: i64, mapping_sets: &Vec<Vec<Mapping>>) -> i64 {
    let mut res = number;

    for mapping in mapping_sets {
        res = pass_through_mapping(res, mapping);
    }

    res
}

fn pass_through_mapping(number: i64, mapping: &Vec<Mapping>) -> i64 {

    for map_part in mapping {
        if number >= map_part.source && number < map_part.source + map_part.length {
            let offset = number - map_part.source;
            return map_part.destination + offset;
        }
    }

    number
}

fn parse_input(input: &str) -> IResult<&str, ProblemInput> {
    let (input, _) = tag("seeds:").parse(input)?;
    let (input, _) = multispace1.parse(input)?;
    let (input, seeds) = separated_list1(multispace1, map_res(digit1, str::parse::<i64>)).parse(input)?;
    let (input, _) = multispace1.parse(input)?;
    let (input , mapping_sets) = separated_list1(multispace1, parse_mapping).parse(input)?;

    Ok((input, ProblemInput {
        seeds,
        mapping_sets
    }))
}

fn parse_mapping(input: &str) -> IResult<&str, Vec<Mapping>> {
    let (input, _) = alpha1.parse(input)?;
    let (input, _) = tag("-to-").parse(input)?;
    let (input, _) = alpha1.parse(input)?;
    let (input, _) = tag(" map:").parse(input)?;
    let (input, _) = newline.parse(input)?;

    let (input, mapping_parts) = separated_list1(newline, parse_mapping_part).parse(input)?;

    Ok((input, mapping_parts))
}

fn parse_mapping_part(input: &str) -> IResult<&str, Mapping> {
    let (input, destination) = map_res(digit1, str::parse::<i64>).parse(input)?;
    let (input, _) = multispace1.parse(input)?;
    let (input, source) = map_res(digit1, str::parse::<i64>).parse(input)?;
    let (input, _) = multispace1.parse(input)?;
    let (input, length) = map_res(digit1, str::parse::<i64>).parse(input)?;

    Ok((input, Mapping {
        destination,
        source,
        length
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

#[derive(Debug)]
struct ProblemInput {
    seeds: Vec<i64>,
    mapping_sets: Vec<Vec<Mapping>>
}

#[derive(Debug)]
struct Mapping {
    destination: i64,
    source: i64,
    length: i64
}
