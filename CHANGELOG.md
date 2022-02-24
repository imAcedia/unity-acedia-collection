# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.3.0] - 2022-02-24

### Added
- TryCast method now also supports structs.

### Changed
- CoroutineHandler can now be created without the initial Coroutine being set.
- CoroutineHandler can now be set to start manually instead of automatically starting after construction.

## [0.2.1] - 2021-11-13

### Fixed
- Fixed DynamicSlider not working with int fields


## [0.2.0] - 2021-09-06

### Added
- Started using Changelog
- Added Pivot2D struct
- Pivot2D attribute can now draws Pivot2D structs

### Changed
- RectColliderSetter now uses the new Pivot2D struct
- Changed CardinalDirectionAttribute to be drawn similar to Pivot2DAttribute instead
