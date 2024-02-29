use std::collections::HashMap;
use std::fs::File;
use std::io::Read;
use colored::Colorize;

#[derive(Debug, Clone, Copy, PartialEq, Eq)]
enum PatternItem {
    Ash,
    Rocks,
}

impl PatternItem {
    fn from_char(chr: char) -> Result<PatternItem, String> {
        match chr {
            '.' => Ok(PatternItem::Ash),
            '#' => Ok(PatternItem::Rocks),
            _ => Err(format!("Could not convert the character {}", chr))
        }
    }

    fn to_char(&self) -> char {
        match self {
            PatternItem::Ash => '.',
            PatternItem::Rocks => '#',
        }
    }

    fn reverse(&self) -> PatternItem {
        match self {
            PatternItem::Ash => PatternItem::Rocks,
            PatternItem::Rocks => PatternItem::Ash,
        }
    }
}

type Position = (usize, usize);
#[derive(Debug, Clone)]
struct Pattern {
    width: usize,
    height: usize,
    mapping: HashMap<Position, PatternItem>,
}

impl Pattern {
    fn print(&self, highlight: Option<Position>, show_mirror_lines: bool) {
        let column_mirrors = find_mirrored_columns(self).unwrap();
        let row_mirrors = find_mirrored_rows(self).unwrap();

        for j in 0..self.height {
            if show_mirror_lines && row_mirrors.contains(&j) {
                for i in 0..self.width {
                    print!("-");
                }
                println!();
            }

            for i in 0..self.width {
                let item = self.mapping.get(&(i, j)).unwrap();

                if show_mirror_lines && column_mirrors.contains(&i) {
                    print!("|");
                }

                if let Some((row, col)) = highlight {
                    if row == i && col == j {
                        print!("{}", item.to_char().to_string().on_blue());
                    } else {
                        print!("{}", item.to_char());
                    }
                } else {
                    print!("{}", item.to_char());
                }
            }
            println!();
        }
    }
}

fn main() -> Result<(), String> {
    let start = std::time::Instant::now();
    let file_name = "../input.txt";
    let file_contents = read_input_file(file_name)?;
    let patterns = parse_input(&file_contents)?;
    println!("Reading and parsing done in {:#?}", start.elapsed());
    println!();

    let part_one_start = std::time::Instant::now();
    let part_one_res = part_one(&patterns)?;
    let part_one_time = part_one_start.elapsed();

    let part_two_start = std::time::Instant::now();
    let part_two_res = part_two(&patterns)?;
    let part_two_time = part_two_start.elapsed();

    println!("Part 1 res: {} ({:#?})", part_one_res, part_one_time);
    println!("Part 2 res: {} ({:#?})", part_two_res, part_two_time);

    Ok(())
}

fn part_one(patterns: &[Pattern]) -> Result<i64, String> {
    let mut res: i64 = 0;

    for pattern in patterns {
        let columns = find_mirrored_columns(pattern)?;
        let rows = find_mirrored_rows(pattern)?;

        let tmp = columns.iter().sum::<usize>() + 100 * rows.iter().sum::<usize>();
        res += tmp as i64;
    }

    Ok(res)
}

fn find_mirrored_columns(pattern: &Pattern) -> Result<Vec<usize>, String> {
    let mut columns = Vec::new();

    for i in 1..pattern.width {
        let closest = usize::min(i, pattern.width - i);

        let mut pairs: Vec<(usize, usize)> = Vec::new();
        for j in 1..=closest {
            pairs.push((i - j, i + j - 1));
        }

        let mirrored = pairs
            .iter()
            .try_fold(true, |acc, &pair| {
                check_if_columns_match(pattern, pair).map(|x| acc && x)
            })?;

        if mirrored {
            columns.push(i)
        }
    }

    Ok(columns)
}

fn check_if_columns_match(pattern: &Pattern, (row1, row2): (usize, usize)) -> Result<bool, String> {
    for i in 0..pattern.height {
        let item1 = pattern.mapping.get(&(row1, i)).ok_or(format!("Could not find position {:?} in mapping", (row1, i)))?;
        let item2 = pattern.mapping.get(&(row2, i)).ok_or(format!("Could not find position {:?} in mapping", (row2, i)))?;
        if item1 != item2 {
            return Ok(false);
        }
    }

    Ok(true)
}

fn find_mirrored_rows(pattern: &Pattern) -> Result<Vec<usize>, String> {
    let mut rows = Vec::new();

    for i in 1..pattern.height {
        let closest = usize::min(i, pattern.height - i);

        let mut pairs: Vec<(usize, usize)> = Vec::new();
        for j in 1..=closest {
            pairs.push((i - j, i + j - 1));
        }

        let mirrored = pairs
            .iter()
            .try_fold(true, |acc, &pair| {
                check_if_rows_match(pattern, pair).map(|x| acc && x)
            })?;

        if mirrored {
            rows.push(i)
        }
    }

    Ok(rows)
}

fn check_if_rows_match(pattern: &Pattern, (col1, col2): (usize, usize)) -> Result<bool, String> {
    for i in 0..pattern.width {
        let item1 = pattern.mapping.get(&(i, col1)).ok_or(format!("Could not find position {:?} in mapping", (i, col1)))?;
        let item2 = pattern.mapping.get(&(i, col2)).ok_or(format!("Could not find position {:?} in mapping", (i, col2)))?;
        if item1 != item2 {
            return Ok(false);
        }
    }

    Ok(true)
}

fn part_two(patterns: &[Pattern]) -> Result<i64, String> {
    let mut patterns = patterns.to_vec().clone();

    let mut res: i64 = 0;
    for pattern in patterns.iter_mut() {
        let before_columns = find_mirrored_columns(&pattern)?;
        let before_rows = find_mirrored_rows(&pattern)?;

        let position_to_change = find_position_to_change(pattern)?;
        let current_val = pattern.mapping.get(&position_to_change).ok_or(format!("Could not find position {:?}", position_to_change))?;
        *pattern.mapping.get_mut(&position_to_change).unwrap() = current_val.reverse();

        let after_columns = find_mirrored_columns(&pattern)?;
        let after_rows = find_mirrored_rows(&pattern)?;

        for col in after_columns {
            if before_columns.contains(&col) {
                continue;
            }

            res += col as i64;
        }

        for row in after_rows {
            if before_rows.contains(&row) {
                continue;
            }

            res += 100 * row as i64;
        }
    }

    Ok(res)
}

fn find_position_to_change(pattern: &Pattern) -> Result<Position, String> {
    let mut column_differences = Vec::new();
    let mut row_differences = Vec::new();

    for i in 1..pattern.width {
        let closest = usize::min(i, pattern.width - i);

        let mut pairs: Vec<(usize, usize)> = Vec::new();
        for j in 1..=closest {
            pairs.push((i - j, i + j - 1));
        }

        let mut differences = Vec::new();
        for pair in pairs {
            let tmp = find_column_differences(pattern, pair)?;
            differences.extend(tmp);
        }

        if differences.len() == 1 {
            column_differences.extend(differences);
        }
    }

    for i in 1..pattern.height {
        let closest = usize::min(i, pattern.height - i);

        let mut pairs: Vec<(usize, usize)> = Vec::new();
        for j in 1..=closest {
            pairs.push((i - j, i + j - 1));
        }

        let mut differences = Vec::new();
        for pair in pairs {
            let tmp = find_row_differences(pattern, pair)?;
            differences.extend(tmp);
        }

        if differences.len() == 1 {
            row_differences.extend(differences);
        }
    }

    assert_eq!(column_differences.len() + row_differences.len(), 1);

    if column_differences.len() == 1 {
        Ok(column_differences[0])
    } else {
        Ok(row_differences[0])
    }
}

fn find_column_differences(pattern: &Pattern, (row1, row2): (usize, usize)) -> Result<Vec<Position>, String> {
    let mut differences = Vec::new();

    for i in 0..pattern.height {
        let item1 = pattern.mapping.get(&(row1, i)).ok_or(format!("Could not find position {:?} in mapping", (row1, i)))?;
        let item2 = pattern.mapping.get(&(row2, i)).ok_or(format!("Could not find position {:?} in mapping", (row2, i)))?;
        if item1 != item2 {
            differences.push((row1, i));
        }
    }

    Ok(differences)
}

fn find_row_differences(pattern: &Pattern, (col1, col2): (usize, usize)) -> Result<Vec<Position>, String> {
    let mut differences = Vec::new();

    for i in 0..pattern.width {
        let item1 = pattern.mapping.get(&(i, col1)).ok_or(format!("Could not find position {:?} in mapping", (i, col1)))?;
        let item2 = pattern.mapping.get(&(i, col2)).ok_or(format!("Could not find position {:?} in mapping", (i, col2)))?;
        if item1 != item2 {
            differences.push((i, col1));
        }
    }

    Ok(differences)
}

fn parse_input(input: &str) -> Result<Vec<Pattern>, String> {
    let mut patterns: Vec<Pattern> = Vec::new();

    let pattern_strings = input.split("\n\n");

    for pattern_string in pattern_strings {
        let lines: Vec<_> = pattern_string.lines().filter(|x| !x.is_empty()).collect();

        let height = lines.len();
        let width = lines.first().ok_or("Pattern has no lines")?.len();

        let mut mapping: HashMap<Position, PatternItem> = HashMap::new();

        for (row, line) in lines.iter().enumerate() {
            for (col, chr) in line.chars().enumerate() {
                mapping.insert((col, row), PatternItem::from_char(chr)?);
            }
        }

        patterns.push(Pattern {
            height,
            width,
            mapping,
        });
    }

    Ok(patterns)
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

