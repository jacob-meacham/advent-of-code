defmodule SafeReports do
  @moduledoc "Day 2"

  defp parse_input(input) do
    input
    |> String.split("\n", trim: true)
    |> Enum.map(fn t ->
        String.split(t, " ", trim: true)
        |> Enum.map(&String.to_integer/1)
      end)
  end

  def part1(input) do
    parse_input(input) |>
      Enum.map(fn row ->
        safe?(row)
      end)
      |> Enum.count(& &1)
  end

  defp valid?(a, b, prev_diff) do
    next_diff = a - b
    abs(next_diff) <= 3 && (prev_diff == nil || (next_diff * prev_diff) > 0)
  end

  defp safe?([first | row]) do
    {_, _, valid} = row |> Enum.reduce({first, nil, true}, fn
        _, {_, _, false} -> {0, 0, false}
        v, {prev, prev_diff, _} -> {v, v - prev, valid?(v, prev, prev_diff)}
      end)
    valid
  end

  def part2(input) do
    parse_input(input) |>
      Enum.map(fn row ->
        [row | Enum.map(0..(length(row) - 1), &List.delete_at(row, &1))]
        |> Enum.any?(&safe?/1)
      end)
      |> Enum.count(& &1)
  end
end

{:ok, contents} = File.read("lib/day2/input.txt")
IO.puts(SafeReports.part1(contents))
IO.puts(SafeReports.part2(contents))


Benchee.run(%{
              day2_part1: fn -> contents |> SafeReports.part1() end,
              day2_part2: fn -> contents |> SafeReports.part2() end,
              day2_total: fn ->
                SafeReports.part1(contents)
                SafeReports.part2(contents)
              end}, warmup: 2,
time: 3)
