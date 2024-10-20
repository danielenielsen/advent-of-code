use std::cmp::max;
use std::fs::File;
use std::io::Read;

use nom::{
    IResult,
    bytes::complete::tag,
    Parser
};
use nom::character::complete::{alpha1, digit1, space1};
use nom::combinator::map_res;
use nom::sequence::separated_pair;
use nom::multi::separated_list1;


fn main() -> Result<(), String> {
    let file_name = "../input.txt";
    let file_contents = read_input_file(file_name)?;
    let games = parse_string_to_representation(&file_contents)?;

    let part_one_res = part_one(&games);
    let part_two_res = part_two(&games);

    println!("Part one sum: {}", part_one_res);
    println!("Part one sum: {}", part_two_res);

    Ok(())
}

fn part_one(games: &Vec<Game>) -> i32 {
    let mut sum = 0;
    for game in games {
        let mut valid_round = true;
        for round in &game.rounds {
            for roll in round {
                match roll {
                    Roll::Red(val) => {
                        if *val > 12 {
                            valid_round = false;
                        }
                    }
                    Roll::Green(val) => {
                        if *val > 13 {
                            valid_round = false;
                        }
                    }
                    Roll::Blue(val) => {
                        if *val > 14 {
                            valid_round = false;
                        }
                    }
                }
            }
        }

        if valid_round {
            sum += game.id;
        }
    }

    return sum;
}

fn part_two(games: &Vec<Game>) -> i32 {
    let mut sum = 0;
    for game in games {

        let mut max_red = 0;
        let mut max_green = 0;
        let mut max_blue = 0;

        for round in &game.rounds {
            for roll in round {
                match roll {
                    Roll::Red(val) => {
                        max_red = max(max_red, *val);
                    }
                    Roll::Green(val) => {
                        max_green = max(max_green, *val);
                    }
                    Roll::Blue(val) => {
                        max_blue = max(max_blue, *val);
                    }
                }
            }
        }

        sum += max_red * max_green * max_blue;
    }

    return sum;
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

fn parse_string_to_representation(file_contents: &String) -> Result<Vec<Game>, String> {
    let lines = file_contents.lines().filter(|&s| !s.is_empty());

    let mut games: Vec<Game> = Vec::new();
    for line in lines {
        let game = match parse_line(line) {
            Ok((_, res)) => res,
            Err(err) => return Err(format!("{}", err))
        };

        games.push(game);
    }

    Ok(games)
}

fn parse_line(input: &str) -> IResult<&str, Game> {
    let (input, _) = tag("Game ").parse(input)?;
    let (input, id) = map_res(digit1, str::parse::<i32>).parse(input)?;
    let (input, _) = tag(": ")(input)?;
    let (input, rounds) = separated_list1(tag("; "), parse_round).parse(input)?;

    let game = Game {
        id,
        rounds
    };

    Ok((input, game))
}

fn parse_round(input: &str) -> IResult<&str, Vec<Roll>> {
    separated_list1(tag(", "), parse_roll)(input)
}

fn parse_roll(input: &str) -> IResult<&str, Roll> {
    let (input, (amount, color)) = separated_pair(map_res(digit1, str::parse::<i32>), space1, alpha1)(input)?;

    if amount < 1 {
        panic!("Amount less than 1")
    }

    let roll = match color {
        "red" => Roll::Red(amount),
        "green" => Roll::Green(amount),
        "blue" => Roll::Blue(amount),
        _ => panic!("Invalid color")
    };

    Ok((input, roll))
}

#[derive(Debug)]
struct Game {
    pub id: i32,
    pub rounds: Vec<Vec<Roll>>
}

#[derive(Debug)]
enum Roll {
    Red(i32),
    Green(i32),
    Blue(i32)
}
