use std::fs::File;
use std::io::Read;

use std::collections::{
    HashSet,
    HashMap
};

use nom::{
    bytes::complete::tag,
    character::complete::{digit1, multispace1},
    multi::separated_list1,
    combinator::map_res,
    IResult,
    Parser
};


fn main() -> Result<(), String> {
    let file_name = "../input.txt";
    let file_contents = read_input_file(file_name)?;
    let cards = parse_input(&file_contents)?;

    let part_one_res: i32 = part_one(&cards)?;
    let part_two_res: i32 = part_two(&cards)?;

    println!("Part 1 res: {}", part_one_res);
    println!("Part 2 res: {}", part_two_res);

    Ok(())
}

fn part_one(cards: &Vec<Card>) -> Result<i32, String> {
    let mut res = 0;

    for card in cards {
        let correct_numbers = card.get_correct_number_num();

        if correct_numbers > 0 {
            res += 1 << (correct_numbers - 1);
        }
    }

    Ok(res)
}

fn part_two(cards: &Vec<Card>) -> Result<i32, String> {
    let mut count_hashmap: HashMap<i32, i32> = HashMap::from_iter(cards.iter().map(|card| (card.id, 1)));

    for card in cards {
        let correct_numbers = card.get_correct_number_num();

        let start = card.id + 1;
        let end = start + correct_numbers;

        let current_card_count = *count_hashmap.get(&card.id).unwrap();

        for i in start..end {
            let i = i;

            *count_hashmap.get_mut(&i).unwrap() += current_card_count;
        }
    }

    Ok(count_hashmap.values().sum())
}

fn parse_input(input: &str) -> Result<Vec<Card>, String> {
    let lines: Vec<_> = input.lines().filter(|&x| !x.is_empty()).collect();

    let mut cards = Vec::new();
    for &line in lines.iter() {
        let (_, card) = match parse_card(line) {
            Ok(val) => val,
            Err(_) => return Err(format!("Could not parse line: {}",line))
        };

        cards.push(card);
    }

    Ok(cards)
}

fn parse_card(input: &str) -> IResult<&str, Card> {
    let (input, _) = tag("Card").parse(input)?;
    let (input, _) = multispace1.parse(input)?;
    let (input, id) = map_res(digit1, str::parse::<i32>).parse(input)?;
    let (input, _) = tag(":").parse(input)?;
    let (input, _) = multispace1.parse(input)?;
    let (input, numbers) = separated_list1(multispace1, map_res(digit1, str::parse::<i32>)).parse(input)?;
    let (input, _) = multispace1.parse(input)?;
    let (input, _) = tag("|").parse(input)?;
    let (input, _) = multispace1.parse(input)?;
    let (input, winning_numbers) = separated_list1(multispace1, map_res(digit1, str::parse::<i32>)).parse(input)?;

    Ok((input, Card {
        id,
        numbers,
        winning_numbers
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
struct Card {
    id: i32,
    numbers: Vec<i32>,
    winning_numbers: Vec<i32>
}

impl Card {
    pub fn get_correct_number_num(&self) -> i32 {
        let combined: Vec<_> = self.numbers.iter().chain(&self.winning_numbers).collect();
        let combined_size = combined.len();

        let hashset: HashSet<_> = combined.iter().collect();
        let new_size = hashset.len();

        (combined_size - new_size) as i32
    }
}
