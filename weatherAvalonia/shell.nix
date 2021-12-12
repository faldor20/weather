{ pkgs ? import <nixpkgs> {} ,lib ? pkgs.lib}:

pkgs.mkShell rec {
  buildInputs = with pkgs;[
    hello
    mono6
    fontconfig
    xorg.libX11
    xorg.libX11.dev
    zlib
  ];
  LD_LIBRARY_PATH = "${lib.makeLibraryPath buildInputs}";
}
