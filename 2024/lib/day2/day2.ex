defmodule Day2 do
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

  # defp is_safe_b([first | row]) do
  #   t = row |> Enum.reduce({first, nil, nil, false, true}, fn
  #       _, {_, _, _, true, false} ->
  #         IO.inspect(true)
  #         {0, 0, 0, true, false}
  #       v, {prev, prev_prev, prev_diff, used_lookback, _} ->
  #         IO.inspect({v, prev, prev_prev, prev_diff, used_lookback, true})
  #       next_diff = v - prev
  #       # TODO: Cleanup ugly
  #       other_diff = case (prev_prev) do
  #         nil -> 0
  #         _ -> v - prev_prev
  #       end
  #       valid = is_valid(v, prev, prev_diff)
  #       # TODO: Need to handle the case where the first match is invalid
  #       cond do
  #         prev_prev == nil && !valid ->
  #           {v, prev, next_diff, true, true}
  #         true ->
  #           case {valid, used_lookback} do
  #             {true, _} -> {v, prev, next_diff, used_lookback, valid}
  #             # next_diff also needs to change in this case...
  #             {false, false} -> {v, prev_prev, other_diff, true, true}
  #             {false, true} -> {0, 0, 0, false, false}
  #           end
  #       end
  #     end)
  #     IO.inspect(t)
  #     {_, _, _, _, valid} = t
  #     valid
  # end

  def part2(input) do
    parse_input(input) |>
      Enum.map(fn row ->
        [row | Enum.map(0..(length(row) - 1), &List.delete_at(row, &1))]
        |> Enum.any?(&safe?/1)
      end)
      |> Enum.count(& &1)
  end

  # def part2_b(input) do
  #   parse_input(input) |>
  #     Enum.map(fn row ->
  #       is_safe_b?(row)
  #     end)
  #     |> Enum.count(& &1)
  # end
end

{:ok, contents} = File.read("lib/day2/input.txt")
IO.puts(Day2.part1(contents))
IO.puts(Day2.part2(contents))


Benchee.run(%{part_1: fn -> contents |> Day2.part1() end,
            part_2: fn -> contents |> Day2.part2() end,
            #  part_2b: fn -> contents |> Day2.part2_b() end
             }, warmup: 2,
time: 5)
