use common::benchmarking::benchmark_test;
use std::collections::HashSet;
use text_io::scan;
use std::iter::FromIterator;
use itertools::Itertools;

// TODO: Rule to dictionary instead of RuleRange
struct Rule {
    name: String,
    range: [(u32, u32); 2]
}

impl Rule {
    fn default() -> Self { Rule { name: "".to_string(), range: [(0, 0), (0, 0)] } }

    fn check(&self, n: u32) -> bool {
        in_range(n, self.range[0]) || in_range(n, self.range[1])
    }

    fn is_ticket_valid(&self, ticket: &[u32]) -> bool {
        ticket.iter().all(|&n| self.check(n))
    }
}

fn in_range(n: u32, range: (u32, u32)) -> bool {
    n >= range.0 && n <= range.1
}

fn test(input: &str) -> (u32, u64) {
    // Just for fun, only a single iterator is allowed (at the end). Otherwise, use only Option methods
    let mut ticket_input = input.split("\n\n");
    let rules: Vec<Rule> = ticket_input.next().unwrap()
        .split('\n')
        .map(|rule_def| {
            let mut rule = Rule::default();
            scan!(rule_def.bytes() => "{}: {}-{} or {}-{}", rule.name, rule.range[0].0, rule.range[0].1, rule.range[1].0, rule.range[1].1);
            rule
    }).collect();
    let num_rules = rules.len();

    let my_ticket = ticket_input.next()
        .and_then(|s| s.lines().nth(1))
        .map(|s| s.split(','))
        .map(|s| s.map(|i| i.parse().unwrap()))
        .expect("Could not parse my ticket!")
        .collect::<Vec<u32>>();

    let other_tickets: Vec<Vec<u32>> = ticket_input.next()
        .map(|s| s.lines().skip(1))
        .map(|s| {
            s.map(|line| {
                line.split(',')
                    .map(|i| i.parse().unwrap())
                    .collect::<Vec<u32>>()
            })
        })
        .expect("Could not parse tickets!")
        .collect();

    // We can create a global range to check against, since the ranges are not disjoint.
    // This would not work with all possible valid inputs
    let global_range = rules.iter()
        .fold((u32::max_value(), 0u32), |global_range, rule| {
            rule.range
                .iter()
                .fold(global_range, |(lmin, lmax), (rmin, rmax)| (lmin.min(*rmin), lmax.max(*rmax)))
        });

    let p1 = other_tickets.iter().map(|ticket_numbers| {
        ticket_numbers.iter().filter(|&&n| global_range.0 > n || n > global_range.1).sum::<u32>()
    }).sum();

    // For each index, we'll keep a list of possible fields.
    let mut possible_rules_by_index: Vec<HashSet<_>> = Vec::new();
    let valid_tickets: Vec<&Vec<_>> = other_tickets.iter()
        .filter(|&ticket| ticket.iter().all(|&n| in_range(n, global_range)))
        .collect();

    // Iterate over each column and test them against each rule to make a list of all rules that could be valid for the field
    (0..num_rules)
        .map(|field_index| valid_tickets.clone().into_iter().flatten()
            .skip(field_index).step_by(num_rules))
        .for_each(|column_values| {
            let all_field_values: Vec<_> = column_values.cloned().collect();
            let possibly_valid_rules = HashSet::from_iter(rules.iter().filter(|&rule| {
                rule.is_ticket_valid(&all_field_values)
            }).map(|r| &r.name));

            possible_rules_by_index.push(possibly_valid_rules);
        });

    // Figure out which rule should be assigned to each. This assumes a single pass through is sufficient.
    // It would be possible to have solvable constraints that don't adhere to this.
    let mut assigned_rules: HashSet<&String> = HashSet::new();
    let p2 = possible_rules_by_index.iter().enumerate()
        .sorted_by(|(_, a), (_, b)| Ord::cmp(&a.len(), &b.len()))
        .filter_map(|(index, possible_rules)| {
            let new_assigned_rules = &assigned_rules.clone();
            let assigned_rule = possible_rules
                .difference(new_assigned_rules)
                .next().unwrap();

            assigned_rules.insert(&assigned_rule);

            if assigned_rule.starts_with("departure") {
                return Some(*my_ticket.get(index).unwrap() as u64)
            }
            None
        }).product();

    (p1, p2)
}

fn main() {
    let mut input = String::new();
    common::input::open_input().read_to_string(&mut input).expect("Could not read input");

    let (p1, p2) = test(&input);

    println!("✅ Part 1: {}", p1);
    println!("✅ Part 2: {}", p2);

    benchmark_test(|| { test(&input); });
 }
