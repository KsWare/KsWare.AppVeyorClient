using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace KsWare.AppVeyorClient.Shared;

public static class StringExtensions {

	/// <summary>
	/// Searches an input string for a substring that matches a regular expression pattern and returns the first occurrence as a single Match object.
	/// </summary>
	/// <param name="s">The string to search for a match.</param>
	/// <param name="pattern">The regular expression pattern to match.</param>
	/// <param name="match">An object that contains information about the match.</param>
	/// <returns><c>true</c> if the regular expression finds a match in the input string.; otherwise, <c>false</c>.</returns>
	public static bool IsMatch(this string s, [RegexPattern] string pattern, [CanBeNull] out Match match) {
		match = null;
		if (s == null) return false;
		match = Regex.Match(s, pattern);
		return match.Success;
	}

	/// <summary>
	/// Searches an input string for a substring that matches a regular expression pattern and returns the first occurrence as a single Match object.
	/// </summary>
	/// <param name="s">The string to search for a match.</param>
	/// <param name="pattern">The regular expression pattern to match.</param>
	/// <param name="options">A bitwise combination of the enumeration values that specify options for matching.</param>
	/// <param name="match">An object that contains information about the match.</param>
	/// <returns><c>true</c> if the regular expression finds a match in the input string.; otherwise, <c>false</c>.</returns>
	public static bool IsMatch(this string s, [RegexPattern] string pattern, RegexOptions options, [CanBeNull] out Match match) {
		match = null;
		if (s == null) return false;
		match = Regex.Match(s, pattern, options);
		return match.Success;
	}

	/// <summary>
	/// Searches an input string for all occurrences of a regular expression and returns all the matches.
	/// </summary>
	/// <param name="s">The string to search for a match.</param>
	/// <param name="pattern">The regular expression pattern to match.</param>
	/// <param name="matches">A collection of the Match objects found by the search. If no matches are found, the method returns an empty collection object.</param>
	/// <returns><c>true</c> if the regular expression finds any matches in the input string.; otherwise, <c>false</c>.</returns>
	public static bool IsMatches(this string s, [RegexPattern] string pattern, [CanBeNull] out MatchCollection matches) {
		matches = null;
		if (s == null) return false;
		matches = Regex.Matches(s, pattern);
		return matches.Count > 0;
	}

	/// <summary>
	/// Searches an input string for all occurrences of a regular expression and returns all the matches.
	/// </summary>
	/// <param name="s">The string to search for a match.</param>
	/// <param name="pattern">The regular expression pattern to match.</param>
	/// <param name="options">A bitwise combination of the enumeration values that specify options for matching.</param>
	/// <param name="matches">A collection of the Match objects found by the search. If no matches are found, the method returns an empty collection object.</param>
	/// <returns><c>true</c> if the regular expression finds any matches in the input string.; otherwise, <c>false</c>.</returns>
	public static bool IsMatches(this string s, [RegexPattern] string pattern, RegexOptions options, [CanBeNull] out MatchCollection matches) {
		matches = null;
		if (s == null) return false;
		matches = Regex.Matches(s, pattern, options);
		return matches.Count > 0;
	}
}
