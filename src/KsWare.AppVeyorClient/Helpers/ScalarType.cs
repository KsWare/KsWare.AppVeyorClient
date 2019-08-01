namespace KsWare.AppVeyorClient.Helpers
{
	/// <summary>
	/// ScalarType 
	/// </summary>
	/// <remarks>More info online: https://yaml-multiline.info/ </remarks>
	public enum ScalarType
	{
		None,

		/// <summary>
		/// Plain
		/// </summary>
		Plain             = 0x101,

		/// <summary>
		/// Flow Scalars Single-quoted
		/// </summary>
		FlowSingleQuoted = 0x201,

		/// <summary>
		/// Flow Scalars Double-quoted 
		/// </summary>
		FlowDoubleQuoted = 0x202,

		/// <summary>
		/// Block Scalar Style. Keep newlines (literal). Single newline at end (clip) <c>|</c>
		/// </summary>
		BlockLiteral = 0x310,

		/// <summary>
		/// Block Scalar Style. Keep newlines (literal). All newlines from end (keep) <c>|+</c>
		/// </summary>
		BlockLiteralKeep = 0x311,

		/// <summary>
		/// Block Scalar Style. Keep newlines (literal). No newline at end (strip) <c>|-</c>
		/// </summary>
		BlockLiteralStrip = 0x312,

		/// <summary>
		/// Block Scalar Style. Replace newlines with spaces (folded). Single newline at end (clip) <c>&gt;</c>
		/// </summary>
		BlockFolded = 0x320,

		/// <summary>
		/// Block Scalar Style. Replace newlines with spaces (folded). All newlines from end (keep) <c>&gt;+</c>
		/// </summary>
		BlockFoldedKeep = 0x321,

		/// <summary>
		/// Block Scalar Style. Replace newlines with spaces (folded). No newline at end (strip) <c>&gt;-</c>
		/// </summary>
		BlockFoldedStrip = 0x322,
	}
}