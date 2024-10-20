use std::fs::File;
use std::io::Read;

use num::integer::lcm;

use std::collections::HashMap;
use std::rc::Rc;

use nom::{
    bytes::complete::tag,
    character::complete::{alpha1, newline, multispace1, one_of},
    multi::{many1, separated_list1},
    combinator::map_res,
    IResult,
    Parser
};

fn main() -> Result<(), String> {
    let start = std::time::Instant::now();
    let file_name = "../input.txt";
    let file_contents = read_input_file(file_name)?;

    let parse_information = match parse_input(&file_contents) {
        Ok((_, val)) => val,
        Err(err) => return Err(err.to_string())
    };

    let part_one_res = part_one(&parse_information)?;
    let part_two_res = part_two(&parse_information)?;

    println!("Part 1 res: {}", part_one_res);
    println!("Part 2 res: {}", part_two_res);

    println!("Elapsed time: {:#?}", start.elapsed());

    Ok(())
}

fn part_one(parse_information: &ParseInformation) -> Result<i32, String> {
    let new_location_map = get_direction_hashmap(parse_information);

    let mut current: Rc<str> = Rc::from("AAA");
    let mut steps = 0;

    for direction in parse_information.get_infinite_direction_iter() {
        if current.get(0..3).unwrap() == "ZZZ" {
            break;
        }

        current = new_location_map.get(&(current, direction)).unwrap().clone();
        steps += 1;
    }

    Ok(steps)
}

fn part_two(parse_information: &ParseInformation) -> Result<i64, String> {
    let new_location_map = get_direction_hashmap(parse_information);

    let start_values = parse_information.nodes
        .iter()
        .map(|x| x.id.clone())
        .filter(|x| x.chars().nth(2).unwrap() == 'A')
        .collect::<Vec<_>>();

    let mut step_counts = Vec::new();

    for val in start_values {
        let mut current = val;
        let mut steps = 0;

        for direction in parse_information.get_infinite_direction_iter() {
            if current.chars().nth(2).unwrap() == 'Z' {
                step_counts.push(steps);
                break;
            }

            current = new_location_map.get(&(current, direction)).unwrap().clone();
            steps += 1;
        }
    }

    Ok(step_counts.iter().fold(1, |acc, val| lcm(acc, *val)))
}

fn get_direction_hashmap(parse_information: &ParseInformation) -> HashMap<(Rc<str>, Direction), Rc<str>> {
    let mut new_location_map: HashMap<(Rc<str>, Direction), Rc<str>> = HashMap::new();

    for node in &parse_information.nodes {
        new_location_map.insert((node.id.clone(), Direction::Left), node.left.clone());
        new_location_map.insert((node.id.clone(), Direction::Right), node.right.clone());
    }

    return new_location_map;
}

fn parse_input(input: &str) -> IResult<&str, ParseInformation> {
    let (input, directions) = many1(map_res(one_of("LR"), char_to_direction)).parse(input)?;
    let (input, _) = newline.parse(input)?;
    let (input, _) = newline.parse(input)?;
    let (input, nodes) = separated_list1(newline, parse_node).parse(input)?;

    Ok((input, ParseInformation {
        directions,
        nodes
    }))
}

fn parse_node(input: &str) -> IResult<&str, Node> {
    let (input, id) = alpha1.parse(input)?;
    let (input, _) = multispace1.parse(input)?;
    let (input, _) = tag("=").parse(input)?;
    let (input, _) = multispace1.parse(input)?;
    let (input, _) = tag("(").parse(input)?;
    let (input, left) = alpha1.parse(input)?;
    let (input, _) = tag(", ").parse(input)?;
    let (input, right) = alpha1.parse(input)?;
    let (input, _) = tag(")").parse(input)?;

    Ok((input, Node {
        id: Rc::from(id),
        left: Rc::from(left),
        right: Rc::from(right)
    }))
}

fn char_to_direction(c: char) -> Result<Direction, String> {
    match c {
        'L' => Ok(Direction::Left),
        'R' => Ok(Direction::Right),
        _ => Err(format!("Could not convert the char \"{}\" to a direction", c))
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
struct ParseInformation {
    directions: Vec<Direction>,
    nodes: Vec<Node>
}

impl ParseInformation {
    fn get_infinite_direction_iter(&self) -> DirectionInfiniteIterator {
        DirectionInfiniteIterator::new(self.directions.clone())
    }
}

struct DirectionInfiniteIterator {
    data: Vec<Direction>,
    index: usize
}

impl DirectionInfiniteIterator {
    fn new(data: Vec<Direction>) -> Self {
        DirectionInfiniteIterator {
            data,
            index: 0
        }
    }
}

impl Iterator for DirectionInfiniteIterator {
    type Item = Direction;

    fn next(&mut self) -> Option<Self::Item> {

        if self.index >= self.data.len() {
            self.index = 0;
        }

        let result = self.data[self.index].clone();
        self.index += 1;

        Some(result)
    }
}

#[derive(Debug, Clone, Copy, PartialEq, Eq, Hash)]
enum Direction {
    Left,
    Right
}

#[derive(Debug)]
struct Node {
    id: Rc<str>,
    left: Rc<str>,
    right: Rc<str>
}

