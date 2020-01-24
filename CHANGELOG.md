# Changelog
All notable changes to this package are documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [2.0.0] - 2020-01-24
### Added
- `BaseGUI` for custom Decal shaders
- Full Summary for all public API
- Angle and angle falloff controls
- Layer masking
- Sorting order

### Changed
- Moved to package
- Refine `DecalSystem` API
- Refine and consolidate shaders
- Move pooling to use `kPooling`
- Rename `ScriptableDecal` to `DecalData`
- Serialize material as subasset of `DecalData`
- Draw material GUI on `DecalData` editor
- Move pooling properties to `DecalData`
- Automatically create pools where appropriate
- Improve decal orientation
- Expose projection depth to `DecalData`
- Improve shader GUI
- Serialize foldouts to EditorPrefs
- Converted to Universal render pipeline
- Removed requirements for `Projector` component

## [1.0.0] - 2018-11-17
### Added
- Initial release version.
- Editor and runtime Decal creation and manipulation.
- Optional automatic pooling of Decals created at runtime.
- Supports a range of Decal render modes by default.
- DecalDefinition allows an easy API for defining custom Decals.
- UnityPackage, test cases and demo scene included.
