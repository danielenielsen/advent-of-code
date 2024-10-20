
let read_file filename =
    let ic = open_in filename in
    let length = in_channel_length ic in
    let content = really_input_string ic length in
    close_in ic;
    content

let char_is_num chr =
    match chr with
    | '0' -> true
    | '1' -> true
    | '2' -> true
    | '3' -> true
    | '4' -> true
    | '5' -> true
    | '6' -> true
    | '7' -> true
    | '8' -> true
    | '9' -> true
    | _ -> false

let concatenate_chars c1 c2 =
    let s1 = String.make 1 c1 in
    let s2 = String.make 1 c2 in
    s1 ^ s2

let part_one lines =
    let find_first_num line =
        String.fold_right (fun value acc -> if char_is_num value then value else acc) line ' ' in
    let find_last_num line =
        String.fold_left (fun acc value -> if char_is_num value then value else acc) ' ' line in
    let get_line_num line =
        concatenate_chars (find_first_num line) (find_last_num line) in
    let str_nums = List.map (get_line_num) lines in
    let nums = List.map (int_of_string) str_nums in
    List.fold_left (+) 0 nums

let check_str_for_num str =
    if String.length str = 0 then None
    else let first_chr = str.[0] in
    if char_is_num first_chr then Some(first_chr)
    else if String.starts_with ~prefix:"one" str then Some('1')
    else if String.starts_with ~prefix:"two" str then Some('2')
    else if String.starts_with ~prefix:"three" str then Some('3')
    else if String.starts_with ~prefix:"four" str then Some('4')
    else if String.starts_with ~prefix:"five" str then Some('5')
    else if String.starts_with ~prefix:"six" str then Some('6')
    else if String.starts_with ~prefix:"seven" str then Some('7')
    else if String.starts_with ~prefix:"eight" str then Some('8')
    else if String.starts_with ~prefix:"nine" str then Some('9')
    else None

let rec get_string_sublists str =
    match str with
    | "" -> []
    | _ -> let length = String.length str in
           let str_tail = String.sub str 1 (length - 1) in
           str :: (get_string_sublists str_tail)
    

let part_two lines =
    let fold_helper acc value =
        match (acc, check_str_for_num value) with
        | (_, None) -> acc
        | ((' ', ' '), Some(chr)) -> (chr, chr)
        | ((first, _), Some(chr)) -> (first, chr) in
    let find_first_and_last_numbers line =
        let str_sub_lsts = get_string_sublists line in
        List.fold_left (fold_helper) (' ', ' ') str_sub_lsts in
    let get_line_num line =
        let (first, last) = find_first_and_last_numbers line in
        concatenate_chars first last in
    let str_nums = List.map (get_line_num) lines in
    let nums = List.map (int_of_string) str_nums in
    List.fold_left (+) 0 nums

let () =
    let filename = "../input.txt" in
    let input = (read_file filename) in
    let lines = String.split_on_char '\n' input in
    let lines = List.filter (fun l -> l <> "") lines in
    Printf.printf "%d\n" (part_one lines);
    Printf.printf "%d\n" (part_two lines)

