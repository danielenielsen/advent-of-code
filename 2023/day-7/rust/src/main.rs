use std::fs::File;
use std::io::Read;

use std::cmp::Ordering;

use nom::{
    character::complete::{digit1, multispace1, one_of},
    multi::many1,
    combinator::map_res,
    IResult,
    Parser
};

fn main() -> Result<(), String> {
    let start = std::time::Instant::now();
    let file_name = "../input.txt";
    let file_contents = read_input_file(file_name)?;
    let card_bids = parse_input(&file_contents)?;

    let part_one_res = part_one(card_bids.as_slice())?;
    let part_two_res = part_two(card_bids.as_slice())?;

    println!("Part 1 res: {}", part_one_res);
    println!("Part 2 res: {}", part_two_res);

    println!("Elapsed time: {:#?}", start.elapsed());

    Ok(())
}

fn part_one(card_bids: &[CardBid]) -> Result<i32, String> {
    let mut sorted_card_bids = card_bids.to_vec();
    sorted_card_bids.sort_by(|a, b| part_one_compare_cards(a, b));

    let res = sorted_card_bids.iter().enumerate().map(|(idx, card_bid)| card_bid.bid * (idx as i32 + 1)).sum();
    Ok(res)
}

fn part_one_cards_to_value(cards: &[Card]) -> HandValue {
    assert_eq!(cards.len(), 5);

    let mut counts: [i32; 13] = [0; 13];

    for card in cards.iter() {
        let idx: usize = card.get_num();
        counts[idx] += 1;
    }

    let any_five = counts.iter().any(|&x| x == 5);
    let any_four = counts.iter().any(|&x| x == 4);
    let any_three = counts.iter().any(|&x| x == 3);
    let amount_of_twos = counts.iter().filter(|&&x| x == 2).count();

    if any_five {
        return HandValue::FiveKind;
    }

    if any_four {
        return HandValue::FourKind;
    }

    if any_three && amount_of_twos > 0 {
        return HandValue::FullHouse;
    }

    if any_three {
        return HandValue::ThreeKind;
    }

    if amount_of_twos == 2 {
        return HandValue::TwoPair;
    }

    if amount_of_twos == 1 {
        return HandValue::OnePair
    }

    return HandValue::HighCard;
}

fn part_one_compare_cards(cards1: &CardBid, cards2: &CardBid) -> Ordering {
    let type1 = part_one_cards_to_value(&cards1.cards);
    let type2 = part_one_cards_to_value(&cards2.cards);

    if type1 != type2 {
        return type1.cmp(&type2);
    }

    for (card1, card2) in cards1.cards.iter().zip(&cards2.cards) {
        let cmp = card1.cmp(card2);
        match cmp {
            Ordering::Equal => {},
            _ => return cmp
        }
    }

    unreachable!()
}

fn part_two(card_bids: &[CardBid]) -> Result<i32, String> {
    let mut sorted_card_bids = card_bids.to_vec();
    sorted_card_bids.sort_by(|a, b| part_two_compare_cards(a, b));

    let res = sorted_card_bids.iter().enumerate().map(|(idx, card_bid)| card_bid.bid * (idx as i32 + 1)).sum();
    Ok(res)
}

fn part_two_cards_to_value(cards: &[Card]) -> HandValue {
    assert_eq!(cards.len(), 5);

    let mut counts: [i32; 13] = [0; 13];

    for card in cards.iter() {
        let idx: usize = card.get_num();
        counts[idx] += 1;
    }

    let jokers = counts[9];

    let mut counts_without_jokers = counts
        .iter()
        .enumerate()
        .filter(|(idx, _)| *idx != 9)
        .map(|(idx, &ele)| ele)
        .collect::<Vec<_>>();

    counts_without_jokers.sort();
    counts_without_jokers.reverse();

    let highest = counts_without_jokers.get(0).unwrap();
    let next_highest = counts_without_jokers.get(1).unwrap();

    if highest + jokers == 5 {
        return HandValue::FiveKind;
    }

    if highest + jokers == 4 {
        return HandValue::FourKind;
    }

    if highest + jokers == 3 && next_highest + jokers - (3 - highest) == 2 {
        return HandValue::FullHouse;
    }

    if highest + jokers == 3 {
        return HandValue::ThreeKind;
    }

    if highest + jokers == 2 && next_highest + jokers - (2 - highest) == 2 {
        return HandValue::TwoPair;
    }

    if highest + jokers == 2 {
        return HandValue::OnePair;
    }

    return HandValue::HighCard;
}

fn part_two_compare_cards(cards1: &CardBid, cards2: &CardBid) -> Ordering {
    let type1 = part_two_cards_to_value(&cards1.cards);
    let type2 = part_two_cards_to_value(&cards2.cards);

    if type1 != type2 {
        return type1.cmp(&type2);
    }

    for (card1, card2) in cards1.cards.iter().zip(&cards2.cards) {
        if *card1 == Card::Knight && *card2 == Card::Knight {
            continue;
        }

        if *card1 == Card::Knight {
            return Ordering::Less;
        }

        if *card2 == Card::Knight {
            return Ordering::Greater;
        }

        let cmp = card1.cmp(card2);
        match cmp {
            Ordering::Equal => {},
            _ => return cmp
        }
    }

    unreachable!()
}

fn parse_input(input: &str) -> Result<Vec<CardBid>, String> {
    let lines = input.lines().filter(|&x| !x.is_empty()).collect::<Vec<_>>();

    let mut card_bids = Vec::new();
    for line in lines {
        let card_bid = match parse_cards_bid(line) {
            Ok((_, val)) => val,
            Err(_) => return Err(format!("Could not parse: {}", line))
        };

        card_bids.push(card_bid)
    }

    Ok(card_bids)
}

fn parse_cards_bid(input: &str) -> IResult<&str, CardBid> {
    let (input, cards) = many1(map_res(one_of("23456789TJQKA"), char_to_card)).parse(input)?;
    let (input, _) = multispace1.parse(input)?;
    let (input, bid) = map_res(digit1, str::parse::<i32>).parse(input)?;

    Ok((input, CardBid {
        bid,
        cards
    }))
}

fn char_to_card(c: char) -> Result<Card, String> {
    match c {
        '2' => Ok(Card::Two),
        '3' => Ok(Card::Three),
        '4' => Ok(Card::Four),
        '5' => Ok(Card::Five),
        '6' => Ok(Card::Six),
        '7' => Ok(Card::Seven),
        '8' => Ok(Card::Eight),
        '9' => Ok(Card::Nine),
        'T' => Ok(Card::Ten),
        'J' => Ok(Card::Knight),
        'Q' => Ok(Card::Queen),
        'K' => Ok(Card::King),
        'A' => Ok(Card::Ace),
        _ => Err(format!("\"{}\" is not a valid char character", c))
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

#[derive(Debug, Clone)]
struct CardBid {
    cards: Vec<Card>,
    bid: i32
}

#[derive(Debug, PartialEq, Eq, PartialOrd, Ord, Clone)]
enum Card {
    Two = 0,
    Three = 1,
    Four = 2,
    Five = 3,
    Six = 4,
    Seven = 5,
    Eight = 6,
    Nine = 7,
    Ten = 8,
    Knight = 9,
    Queen = 10,
    King = 11,
    Ace = 12
}

impl Card {
    pub fn get_num(&self) -> usize {
        match &self {
            Card::Two => 0,
            Card::Three => 1,
            Card::Four => 2,
            Card::Five => 3,
            Card::Six => 4,
            Card::Seven => 5,
            Card::Eight => 6,
            Card::Nine => 7,
            Card::Ten => 8,
            Card::Knight => 9,
            Card::Queen => 10,
            Card::King => 11,
            Card::Ace => 12
        }
    }
}

#[derive(Debug, PartialEq, Eq, PartialOrd, Ord)]
enum HandValue {
    HighCard,
    OnePair,
    TwoPair,
    ThreeKind,
    FullHouse,
    FourKind,
    FiveKind
}
