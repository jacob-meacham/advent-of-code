defmodule Advent.Day7.BridgeRepair do
  use Advent.Day, no: 7
  @moduledoc "Day 7"

  defp parse_input(input) do
    input
    |> String.split("\n", trim: true)
    |> Enum.map(&parse_line/1)
  end

  defp parse_line(line) do
    [sol, xs] = String.split(line, ":", trim: true)

    ops = xs
    |> String.split(" ", trim: true)
    |> Enum.map(&String.to_integer/1)

    {String.to_integer(sol), ops}
  end

  defp concat(a, b) do
    String.to_integer("#{a}#{b}")
  end

  defp forward_solve({lhs, cur, [x]}) do
    # Case where only one element is left
    cond do
      lhs == concat(cur, x) -> true
      lhs == cur + x -> true
      lhs == cur * x -> true
      true -> false
    end
  end

  defp forward_solve({lhs, cur, [x, y | tail] = _rest}) do
    next_list = if tail == [], do: [y], else: [y | tail]

    case lhs - cur do
      0 -> true
      _ when lhs - cur < 0 -> false
      _ when lhs - cur > 0 ->
       [
        {lhs, cur + x, next_list},
        {lhs, cur * x, next_list},
        {lhs, concat(cur, x), next_list}
       ]
       |> Enum.any?(&forward_solve/1)
    end
  end

  defp solve({lhs, [lhs]}), do: true
  defp solve({_lhs, [_]}), do: false
  defp solve({_lhs, []}), do: false
  defp solve({lhs, [x | tail] = _rest}) do
    []
    |> maybe_add_subtraction(lhs, x, tail)
    |> maybe_add_division(lhs, x, tail)
    |> Enum.any?(&solve/1)
  end

  defp maybe_add_subtraction(possible, lhs, x, next_list) do
    if lhs - x >= 0 do
      [{lhs - x, next_list} | possible]
    else
      possible
    end
  end

  defp maybe_add_division(possible, lhs, x, next_list) do
    if rem(lhs, x) == 0 do
      [{div(lhs, x), next_list} | possible]
    else
      possible
    end
  end

  def part1(input) do
    parse_input(input)
    |> Enum.reduce(0, fn {lhs, rhs}, acc ->
      case solve({lhs, Enum.reverse(rhs)}) do
        true ->
          acc + lhs
        false -> acc
      end
    end)
  end

  def part2(input) do
    parse_input(input)
    |> Task.async_stream(fn {lhs, [x|rhs]} ->
      case forward_solve({lhs, x, rhs}) do
        true ->
          lhs
        false -> 0
      end
    end)
    |> Enum.reduce(0, fn {:ok, val}, acc -> acc + val end)
  end

  def run_part1() do input() |> part1() end
  def run_part2() do input() |> part2() end
end
