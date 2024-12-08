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

| Day    | Part 1 (ms) | Part 2 (ms) | Total (ms) | Good? |
|--------|-------------|-------------|------------|-------|
| Day 1  | 0.65        | 0.8         | 1.45       | ✅    |
| Day 2  | 0.58        | 1.0         | 1.58       | ✅    |
| Day 3  | 0.55        | 1.32        | 1.87       | ✅    |
| Day 4  | 11.7        | 7.57        | 19.27      | ✅    |
| Day 5  | 3.23        | 8.57        | 11.8       | ✅    |
| Day 6  | 0.07        | 0.86        | 0.93       | ✅    |
