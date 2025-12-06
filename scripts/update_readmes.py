#!/usr/bin/env python3
"""
Script to update README files with year links and completion badges.
Fetches completion data directly from Advent of Code API.
"""

import argparse
import os
import re
import time
from pathlib import Path
from typing import Dict, List
import requests
from dotenv import load_dotenv

# Base directory (repo root, parent of scripts/)
BASE_DIR = Path(__file__).parent.parent

# Advent of Code base URL
AOC_BASE_URL = "https://adventofcode.com"


def get_session_token() -> str:
    """Get the Advent of Code session token from .env file or environment variable."""
    # Load .env file from repo root
    env_path = BASE_DIR / ".env"
    load_dotenv(env_path)
    
    token = os.getenv("AOC_SESSION")
    if not token:
        raise ValueError(
            "AOC_SESSION not found in .env file or environment variable. "
            "Please set it to your Advent of Code session cookie value."
        )
    return token


def fetch_completion(year: int, session_token: str) -> int:
    """
    Fetch completion count for a given year from Advent of Code leaderboard.
    Returns the number of stars (0-50, where 50 = all 25 days completed).
    """
    url = f"{AOC_BASE_URL}/{year}/leaderboard/self"
    cookies = {"session": session_token}
    headers = {
        "User-Agent": "github.com/jacob/advent-of-code README updater (contact: jacob.e.meacham@gmail.com)"
    }
    
    try:
        response = requests.get(url, cookies=cookies, headers=headers, timeout=10)
        response.raise_for_status()
        
        html = response.text
        
        # Look for star count in the header: <span class="star-count">5*</span>
        match = re.search(r'<span class="star-count">(\d+)\*</span>', html)
        if match:
            star_count = int(match.group(1))
            if 0 <= star_count <= 50:
                return star_count
        
        return 0
        
    except requests.exceptions.RequestException as e:
        print(f"Warning: Could not fetch completion for {year}: {e}")
        return 0


def stars_to_days_completed(stars: int) -> int:
    """
    Convert star count to number of days completed.
    Each day has 2 stars, so we round down.
    """
    return stars // 2


def find_years() -> List[int]:
    """Find all year directories (4-digit numbers)."""
    years = []
    for item in BASE_DIR.iterdir():
        if item.is_dir() and item.name.isdigit() and len(item.name) == 4:
            years.append(int(item.name))
    return sorted(years)


def generate_badge(year: int, stars: int, total_stars: int = 50) -> str:
    """
    Generate a completion badge for a year.
    Uses star count (0-50) where 50 = all 25 days completed.
    """
    if stars == 0:
        return ""
    
    percentage = (stars / total_stars) * 100
    
    # Choose color based on completion
    if percentage == 100:
        color = "brightgreen"
    elif percentage >= 75:
        color = "green"
    elif percentage >= 50:
        color = "yellow"
    elif percentage >= 25:
        color = "orange"
    else:
        color = "red"
    
    # Use shields.io badge format showing stars completed
    return f"![{year}](https://img.shields.io/badge/{year}-{stars}%2F{total_stars}-{color})"


def generate_year_links(years: List[int]) -> str:
    """Generate the year links line."""
    links = [f"[{year}](/{year})" for year in years]
    return " | ".join(links)


def update_main_readme(years: List[int], stars: Dict[int, int]):
    """Update the main README with year links and badges."""
    readme_path = BASE_DIR / "README.md"
    
    # Read existing content
    if readme_path.exists():
        content = readme_path.read_text()
    else:
        content = "# Advent of Code\n"
    
    # Generate year links
    year_links = generate_year_links(years)
    
    # Generate badges
    badges = []
    for year in years:
        year_stars = stars.get(year, 0)
        if year_stars > 0:
            badges.append(generate_badge(year, year_stars))
    
    badge_line = " ".join(badges) if badges else ""
    
    # Update content
    lines = content.split('\n')
    new_lines = []
    
    # Replace or add title
    if lines and lines[0].startswith('#'):
        new_lines.append(lines[0])
        start_idx = 1
    else:
        new_lines.append("# Advent of Code")
        start_idx = 0
    
    # Add year links
    new_lines.append("")
    new_lines.append(year_links)
    
    # Add badges if they exist (right after links)
    if badge_line:
        new_lines.append("")
        new_lines.append(badge_line)
    
    # Keep rest of content (skip old year links line and badges for years we're updating)
    for i in range(start_idx, len(lines)):
        line = lines[i].strip()
        # Skip old year links line
        if '|' in line and any(str(year) in line for year in years):
            continue
        # Skip old badges only for years we're generating new badges for
        if line.startswith('!['):
            # Check if this badge is for a year we're updating
            should_skip = False
            for year in years:
                if stars.get(year, 0) > 0 and str(year) in line:
                    should_skip = True
                    break
            if should_skip:
                continue
        # Skip empty line after title if we already added one
        if i == start_idx and not line:
            continue
        new_lines.append(lines[i])
    
    readme_path.write_text('\n'.join(new_lines).rstrip() + '\n')


def update_year_readme(year: int, years: List[int], stars: int):
    """Update a year's README with year links and badge."""
    readme_path = BASE_DIR / str(year) / "README.md"
    
    if not readme_path.exists():
        return
    
    content = readme_path.read_text()
    lines = content.split('\n')
    new_lines = []
    
    i = 0
    # Keep title
    if i < len(lines) and lines[i].startswith('#'):
        new_lines.append(lines[i])
        i += 1
    
    # Update year links
    year_links = generate_year_links(years)
    
    # Find and replace year links line
    found_links = False
    while i < len(lines):
        line = lines[i]
        # Check if this is a year links line
        if '|' in line and any(str(y) in line for y in years):
            new_lines.append(year_links)
            found_links = True
            i += 1
            # Skip empty line after links if it exists
            if i < len(lines) and not lines[i].strip():
                i += 1
            # Skip old badge if it exists right after links (only if we're generating a new one)
            if stars > 0 and i < len(lines) and lines[i].strip().startswith('!['):
                i += 1
            # Add badge right after links if there's completion
            if stars > 0:
                badge = generate_badge(year, stars)
                new_lines.append("")
                new_lines.append(badge)
            continue
        # Skip old badges that appear before links (only if we're generating a new one)
        if stars > 0 and not found_links and line.strip().startswith('!['):
            i += 1
            continue
        new_lines.append(line)
        i += 1
    
    # If we didn't find a links line, add it after title
    if not found_links:
        # Insert after title
        insert_idx = len([line for line in new_lines if line.startswith('#')])
        new_lines.insert(insert_idx, "")
        new_lines.insert(insert_idx + 1, year_links)
        # Add badge right after links if there's completion
        if stars > 0:
            badge = generate_badge(year, stars)
            new_lines.insert(insert_idx + 2, "")
            new_lines.insert(insert_idx + 3, badge)
    
    readme_path.write_text('\n'.join(new_lines).rstrip() + '\n')


def main():
    """Main function."""
    parser = argparse.ArgumentParser(
        description="Update README files with year links and completion badges."
    )
    parser.add_argument(
        "years",
        type=int,
        nargs="+",
        help="List of years to fetch completion data from Advent of Code (e.g., 2023 2024 2025). "
             "All year READMEs will be updated with links, but only these years will be queried.",
    )
    args = parser.parse_args()
    
    # Get session token
    try:
        session_token = get_session_token()
    except ValueError as e:
        print(f"Error: {e}")
        print("\nTo get your session token:")
        print("1. Log in to https://adventofcode.com")
        print("2. Open browser developer tools (F12)")
        print("3. Go to Application/Storage > Cookies > https://adventofcode.com")
        print("4. Copy the value of the 'session' cookie")
        print("5. Add it to a .env file in the repo root: AOC_SESSION=your_token_here")
        print("   Or set it as an environment variable: export AOC_SESSION=your_token_here")
        return
    
    years_to_fetch = sorted(args.years)
    print(f"Fetching completion data for years: {years_to_fetch}")
    
    # Fetch completion data from Advent of Code (only for specified years)
    stars = {}
    print("\nFetching completion data from Advent of Code...")
    for year in years_to_fetch:
        print(f"Fetching {year}...", end=" ", flush=True)
        star_count = fetch_completion(year, session_token)
        stars[year] = star_count
        print(f"{star_count}/50 stars")
        # Be polite - add a small delay between requests
        time.sleep(0.5)
    
    # Get all years for README updates
    all_years = find_years()
    print(f"\nUpdating READMEs for all years: {all_years}")
    
    # Update main README (with all years for links, but only fetched years have badges)
    print("\nUpdating main README...")
    update_main_readme(all_years, stars)
    
    # Update year READMEs (all years get updated links, fetched years get badges)
    for year in all_years:
        print(f"Updating {year}/README.md...")
        update_year_readme(year, all_years, stars.get(year, 0))
    
    print("\nDone!")


if __name__ == "__main__":
    main()

