use std::fs::File;
use std::io::Read;
use std::collections::{HashMap, HashSet};
use colored::Colorize;


#[derive(Debug, Clone, Copy, PartialEq, Eq)]
enum Direction {
    North,
    South,
    East,
    West
}

impl Direction {
    fn opposite(&self) -> Direction {
        match *self {
            Direction::North => Direction::South,
            Direction::South => Direction::North,
            Direction::East => Direction::West,
            Direction::West => Direction::East
        }
    }

    fn get_delta(&self) -> (i64, i64) {
        match *self {
            Direction::North => (0, -1),
            Direction::East => (1, 0),
            Direction::South => (0, 1),
            Direction::West => (-1, 0)
        }
    }
}

#[derive(Debug, Clone, Copy, PartialEq, Eq)]
enum TileType {
    NorthSouth,
    EastWest,
    NorthEast,
    NorthWest,
    SouthEast,
    SouthWest,
    Ground
}

impl TileType {
    fn can_go(&self, dir: Direction) -> bool {
        match (*self, dir) {
            (TileType::NorthSouth, Direction::North | Direction::South) => true,
            (TileType::EastWest, Direction::East | Direction::West) => true,
            (TileType::NorthEast, Direction::North | Direction::East) => true,
            (TileType::NorthWest, Direction::North | Direction::West) => true,
            (TileType::SouthEast, Direction::South | Direction::East) => true,
            (TileType::SouthWest, Direction::South | Direction::West) => true,
            _ => false
        }
    }

    fn to_char(&self) -> char {
        match *self {
            TileType::NorthSouth => '|',
            TileType::EastWest => '-',
            TileType::NorthEast => 'L',
            TileType::NorthWest => 'J',
            TileType::SouthEast => 'F',
            TileType::SouthWest => '7',
            TileType::Ground => '.'
        }
    }
}

type Position = (i64, i64);

#[derive(Debug)]
struct PipeSystem {
    tile_map: HashMap<Position, TileType>,
    start_position: Position,
    loop_tiles_set: HashSet<Position>,
    width: i64,
    height: i64
}

impl PipeSystem {
    fn print(&self) {
        for j in 0..self.height {
            for i in 0..self.width {
                let tile = self.tile_map.get(&(i, j)).unwrap();
                let tile_char = tile.to_char().to_string();

                if self.start_position == (i, j) {
                    print!("{}", tile_char.bright_blue());
                } else if self.loop_tiles_set.contains(&(i, j)) {
                    print!("{}", tile_char.red());
                } else {
                    print!("{}", tile_char);
                }
            }

            println!();
        }
    }
}

fn main() -> Result<(), String>{
    let start = std::time::Instant::now();
    let file_name = "../input.txt";
    let file_contents = read_input_file(file_name)?;
    let mut pipe_system = make_pipe_system(&file_contents);
    println!("Reading and parsing done in {:#?}", start.elapsed());
    println!();

    let part_one_start = std::time::Instant::now();
    let part_one_res = part_one(&mut pipe_system);
    let part_one_time = part_one_start.elapsed();

    let part_two_start = std::time::Instant::now();
    let part_two_res = part_two(&pipe_system);
    let part_two_time = part_two_start.elapsed();

    println!("Part 1 res: {} ({:#?})", part_one_res, part_one_time);
    println!("Part 2 res: {} ({:#?})", part_two_res, part_two_time);

    Ok(())
}

fn part_one(pipe_system: &mut PipeSystem) -> i64 {
    let mut came_from_dir: Option<Direction> = None;
    let mut current_pos: Position = pipe_system.start_position;

    let directions = vec![Direction::North, Direction::East, Direction::South, Direction::West];

    let mut count = 0;
    'outer: loop {
        let current_tile = pipe_system.tile_map.get(&current_pos).unwrap();

        for dir in &directions {
            if let Some(prev_dir) = came_from_dir {
                if *dir == prev_dir {
                    continue;
                }
            }

            if current_tile.can_go(*dir) {
                let delta = dir.get_delta();
                let new_pos = (current_pos.0 + delta.0, current_pos.1 + delta.1);

                if new_pos == pipe_system.start_position {
                    break 'outer;
                }

                pipe_system.loop_tiles_set.insert(new_pos);
                came_from_dir = Some(dir.opposite());
                current_pos = new_pos;
                count += 1;

                break;
            }
        }
    }

    if count % 2 == 1 {
        count += 1;
    }

    count / 2
}

fn part_two(pipe_system: &PipeSystem) -> i64 { 
    let mut inside_count = 0;
    for j in 0..pipe_system.height {
        let mut inside_tracker = false;
        for i in 0..pipe_system.width {
            let tile = pipe_system.tile_map.get(&(i, j)).unwrap();

            if pipe_system.loop_tiles_set.contains(&(i, j)) {
                if matches!(tile, TileType::NorthSouth | TileType::NorthEast | TileType::NorthWest) {
                    inside_tracker = !inside_tracker;
                }

                continue;
            }

            if inside_tracker {
                inside_count += 1;
            }
        }
    }

    inside_count
}

fn make_pipe_system(file_contents: &str) -> PipeSystem {
    let mut tile_pos_map = HashMap::new();
    let mut start_position: Option<Position> = None;
    
    let height = file_contents.lines().count();
    let width = file_contents.lines().next().unwrap().len();

    for (row, line) in file_contents.lines().enumerate() {
        for (col, chr) in line.chars().enumerate() {
            let row = row as i64;
            let col = col as i64;

            let tile_type = match chr {
                'S' => {
                    start_position = Some((col, row));
                    TileType::Ground
                },
                '|' => TileType::NorthSouth,
                '-' => TileType::EastWest,
                'L' => TileType::NorthEast,
                'J' => TileType::NorthWest,
                'F' => TileType::SouthEast,
                '7' => TileType::SouthWest,
                _ => TileType::Ground,
            };

            tile_pos_map.insert((col, row), tile_type);
        }
    }

    let Some(start_position) = start_position else {
        panic!("Could not find the start tile")
    };

    let can_go_north = tile_pos_map.get(&(start_position.0, start_position.1 - 1)).unwrap_or(&TileType::Ground).can_go(Direction::South);
    let can_go_south = tile_pos_map.get(&(start_position.0, start_position.1 + 1)).unwrap_or(&TileType::Ground).can_go(Direction::North);
    let can_go_east = tile_pos_map.get(&(start_position.0 + 1, start_position.1)).unwrap_or(&TileType::Ground).can_go(Direction::West);
    let can_go_west = tile_pos_map.get(&(start_position.0 - 1, start_position.1)).unwrap_or(&TileType::Ground).can_go(Direction::East);

    let start_pipe_type = match (can_go_north, can_go_south, can_go_east, can_go_west) {
        (true, true, false, false) => TileType::NorthSouth,
        (true, false, true, false) => TileType::NorthEast,
        (true, false, false, true) => TileType::NorthWest,
        (false, true, true, false) => TileType::SouthEast,
        (false, true, false, true) => TileType::SouthWest,
        (false, false, true, true) => TileType::EastWest,
        _ => TileType::Ground
    };

    tile_pos_map.insert(start_position, start_pipe_type);

    PipeSystem {
        tile_map: tile_pos_map,
        start_position,
        loop_tiles_set: HashSet::from([start_position]),
        width: width as i64,
        height: height as i64
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

