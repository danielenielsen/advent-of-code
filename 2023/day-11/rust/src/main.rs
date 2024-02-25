use std::fs::File;
use std::io::Read;

type Position = (i64, i64);

#[derive(Debug)]
struct SpaceInfo {
    galaxies: Vec<Position>,
    empty_rows: Vec<i64>,
    empty_cols: Vec<i64>,
}

fn main() -> Result<(), String> {
    let start = std::time::Instant::now();
    let file_name = "../input.txt";
    let file_contents = read_input_file(file_name)?;
    let space_info = parse_input(&file_contents);
    println!("Reading and parsing done in {:#?}", start.elapsed());
    println!();

    let part_one_start = std::time::Instant::now();
    let part_one_res = part_one(&space_info)?;
    let part_one_time = part_one_start.elapsed();

    let part_two_start = std::time::Instant::now();
    let part_two_res = part_two(&space_info)?;
    let part_two_time = part_two_start.elapsed();

    println!("Part 1 res: {} ({:#?})", part_one_res, part_one_time);
    println!("Part 2 res: {} ({:#?})", part_two_res, part_two_time);

    Ok(())
}

fn part_one(space_info: &SpaceInfo) -> Result<i64, String> {
    let mut sum = 0;

    for i in 0..space_info.galaxies.len() {
        for j in (i + 1)..space_info.galaxies.len() {
            let galaxy_one = space_info.galaxies[i];
            let galaxy_two = space_info.galaxies[j];

            let col_interval = if galaxy_one.0 < galaxy_two.0 { (galaxy_one.0, galaxy_two.0) } else { (galaxy_two.0, galaxy_one.0) };
            let row_interval = if galaxy_one.1 < galaxy_two.1 { (galaxy_one.1, galaxy_two.1) } else { (galaxy_two.1, galaxy_one.1) };

            let empty_cols_between_num = space_info.empty_cols.iter().filter(|&&x| x > col_interval.0 && x < col_interval.1).count() as i64;
            let empty_rows_between_num = space_info.empty_rows.iter().filter(|&&x| x > row_interval.0 && x < row_interval.1).count() as i64;

            let distance = i64::abs(galaxy_one.0 - galaxy_two.0) + i64::abs(galaxy_one.1 - galaxy_two.1) + empty_cols_between_num + empty_rows_between_num;

            sum += distance;
        }
    }

    Ok(sum)
}

fn part_two(space_info: &SpaceInfo) -> Result<i64, String> {
    let mut sum = 0;

    for i in 0..space_info.galaxies.len() {
        for j in (i + 1)..space_info.galaxies.len() {
            let galaxy_one = space_info.galaxies[i];
            let galaxy_two = space_info.galaxies[j];

            let col_interval = if galaxy_one.0 < galaxy_two.0 { (galaxy_one.0, galaxy_two.0) } else { (galaxy_two.0, galaxy_one.0) };
            let row_interval = if galaxy_one.1 < galaxy_two.1 { (galaxy_one.1, galaxy_two.1) } else { (galaxy_two.1, galaxy_one.1) };

            let empty_cols_between_num = space_info.empty_cols.iter().filter(|&&x| x > col_interval.0 && x < col_interval.1).count() as i64;
            let empty_rows_between_num = space_info.empty_rows.iter().filter(|&&x| x > row_interval.0 && x < row_interval.1).count() as i64;

            let distance = i64::abs(galaxy_one.0 - galaxy_two.0) + i64::abs(galaxy_one.1 - galaxy_two.1) + empty_cols_between_num * 999_999 + empty_rows_between_num * 999_999;

            sum += distance;
        }
    }

    Ok(sum)
}

fn parse_input(input: &str) -> SpaceInfo {
    let width = input.lines().next().unwrap().len() as i64;

    let mut galaxies: Vec<Position> = Vec::new();
    let mut empty_rows: Vec<i64> = Vec::new();
    let mut empty_cols: Vec<i64> = (0..width).collect();

    let lines = input.lines();
    for (row, line) in lines.enumerate() {
        let row = row as i64;
        let mut row_empty = true;

        for (col, chr) in line.chars().enumerate() {
            let col = col as i64;

            if chr == '#' {
                galaxies.push((col, row));

                row_empty = false;

                if empty_cols.contains(&col) {
                    let index = empty_cols.iter().position(|&x| x == col).unwrap();
                    empty_cols.remove(index);
                }
            }
        }

        if row_empty {
            empty_rows.push(row);
        }
    }

    SpaceInfo {
        galaxies,
        empty_rows,
        empty_cols
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

