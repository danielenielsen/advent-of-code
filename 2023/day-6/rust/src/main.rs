use std::fs::File;
use std::io::Read;

use nom::{
    bytes::complete::tag,
    character::complete::{digit1, multispace1, newline},
    multi::separated_list1,
    combinator::map_res,
    IResult,
    Parser
};


fn main() -> Result<(), String> {
    let start = std::time::Instant::now();
    let file_name = "../input.txt";
    let file_contents = read_input_file(file_name)?;

    let problem_input = match parse_input(&file_contents) {
        Ok((_, val)) => val,
        Err(_) => return Err("Parsing failed".to_string())
    };

    let part_one_res = part_one(&problem_input)?;
    let part_two_res = part_two(&problem_input)?;

    println!("Part 1 res: {}", part_one_res);
    println!("Part 2 res: {}", part_two_res);

    println!("Elapsed time: {:?}", start.elapsed());

    Ok(())
}

fn part_one(problem_input: &ProblemInput) -> Result<i32, String> {
    let time_distance_tuples: Vec<_> = problem_input.times.iter().zip(problem_input.distances.iter()).collect();

    let mut results = Vec::with_capacity(time_distance_tuples.len());

    for (time, dist) in time_distance_tuples {
        let mut count = 0;

        for i in 0..*time {
            let distance_travelled = i * (time - i);

            if distance_travelled > *dist {
                count += 1;
            }
        }

        results.push(count);
    }

    Ok(results.iter().product())
}

fn part_two(problem_input: &ProblemInput) -> Result<i64, String> {
    let time: i64 = problem_input.times.iter().map(|x| x.to_string()).collect::<String>().parse().unwrap();
    let distance: i64 = problem_input.distances.iter().map(|x| x.to_string()).collect::<String>().parse().unwrap();

    let mut count = 0;
    for i in 0..time {
        let distance_travelled = i * (time - i);

        if distance_travelled > distance {
            count += 1;
        }
    }

    Ok(count)
}

fn parse_input(input: &str) -> IResult<&str, ProblemInput> {
    let (input, _) = tag("Time:").parse(input)?;
    let (input, _) = multispace1.parse(input)?;
    let (input, times) = separated_list1(multispace1, map_res(digit1, str::parse::<i32>)).parse(input)?;
    let (input, _) = newline.parse(input)?;
    let (input, _) = tag("Distance:").parse(input)?;
    let (input, _) = multispace1.parse(input)?;
    let (input, distances) = separated_list1(multispace1, map_res(digit1, str::parse::<i32>)).parse(input)?;

    Ok((input, ProblemInput {
        times,
        distances
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
    times: Vec<i32>,
    distances: Vec<i32>
}

