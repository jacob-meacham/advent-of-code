defmodule Advent.Day2.SafeReports do
  use Advent.Day, no: 2
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
    parse_input(input)
    |> Enum.count(&safe?/1)
  end

  defp valid?(a, b, prev_diff) do
    next_diff = a - b
    abs(next_diff) <= 3 && (prev_diff == nil || (next_diff * prev_diff) > 0)
  end

  defp safe?([first | row]) do
    {_, _, valid} = row
    |> Enum.reduce({first, nil, true}, fn
        _, {_, _, false} -> {0, 0, false}
        v, {prev, prev_diff, _} -> {v, v - prev, valid?(v, prev, prev_diff)}
      end)
    valid
  end

  def part2(input) do
    parse_input(input) |>
      Enum.count(fn row ->
        [row | Enum.map(0..(length(row) - 1), &List.delete_at(row, &1))]
        |> Enum.any?(&safe?/1)
      end)
  end

  def run_part1() do input() |> part1() end
  def run_part2() do input() |> part2() end
end
