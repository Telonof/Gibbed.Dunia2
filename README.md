# Dunia2 Tools
This a toolset designed for Ivory Tower's version of Dunia2 (called Babel) for The Crew and The Crew 2.

The following tools are included and maintained:
- Dunia2 Unpack/Repack: Tools to extract and repack the game's main archive format (.dat/.fat combo).
- Dunia2 ConvertBinaryObject: Converts the game's bin format (fcb) file to .xml and back.
- Dunia2 ConvertBabelDB: Converts the game's database format (.babdb) to .csv.

## Credits
- Gibbed for the original Dunia2 tools.
- Guki & Mono: All filenames and CRC32 variable names.

## Usage
> [!IMPORTANT]
> If working with The Crew 2, change `The Crew` in the projects folder to `The Crew 2` and copy `oo2core_5_win64.dll` from your game folder into the folder of this program.

Unpacking .dat:
`Gibbed.Dunia2.Unpack.exe [-v] <input .fat> [output folder]`
> The .dat associated with the .fat should be in the same folder.
- -v: Provides a verbose output of all files being unpacked.

Repacking .dat:
`Gibbed.Dunia2.Pack.exe [-c] [-v] [--pv 6] [--au <author name>] <output .fat> <input folder>`

- -v: Provides a verbose output of all files being packed.
- -c: Compresses the files with LZO1x or with Oodle for The Crew 2.
- --pv 6: Sets the version of the file to 6 which is needed for The Crew 2.
- --au \<author name>: Integrates a custom author into the files.

Converting .bin->.xml:
`Gibbed.Dunia2.ConvertBinaryObject.exe -e <input .bin> [output .xml]`

Converting .xml->.bin:
`Gibbed.Dunia2.ConvertBinaryObject.exe -i <input .xml> [output .bin]`

Converting .babdb->.csv:
`Dunia2.ConvertBabelDB.exe <input .babdb>`
