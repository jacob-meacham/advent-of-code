# AOC 2024

Elixir edition!

## Framework TODO

Figure out the best way to organize so that it's easy to run a single one, loading input is not part of the benchmarking and it's easy to run all

TODO:

[time: 3]
|> Benchee.init()
|> Benchee.system()
|> Benchee.benchmark("flat_map", fn -> Enum.flat_map(list, map_fun) end)
|> Benchee.benchmark(
  "map.flatten",
  fn -> list |> Enum.map(map_fun) |> List.flatten() end
)
|> Benchee.collect()
|> Benchee.statistics()
|> Benchee.relative_statistics()
|> Benchee.Formatter.output(Benchee.Formatters.Console)

## Timing

| Day    | Part 1 (ms) | Part 2 (ms) | Total (ms) |Good? |
|--------|-------------|-------------|------------|------|
| Day 1  | 0.77        | 0.78        | 1.3        |✅    |
| Day 2  | 0.54        | 0.77        | 1.4        |✅    |
| Day 3  | 0.56        | 1.2         | 1.9        |✅    |
| Day 4  | -           | -           | _          |❌    |
| Day 5  | 3.8         | 8.9         | 14.1       |✅    |
|--------|-------------|-------------|------------|------|
