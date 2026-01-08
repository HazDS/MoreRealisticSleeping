# Changelog
## [1.0.5]

### Fixed
- Product Manager Filter mod compatibility issue

## [1.0.4]

### Changed
- Updated for Schedule I v0.4.2f9 (IL2CPP Main Branch)
- Embedded all assets directly in the DLL (no external asset files needed in UserData folder)

### Fixed
- API compatibility fixes for game namespace migrations (`ScheduleOne.Properties` → `ScheduleOne.Effects`)
- Updated `Player.Arrest()` to `Player.Arrest_Server()` for latest game API
- Fixed ScriptableObject instantiation for effect types

## [1.0.3]

### Added
- **Murdered Event**: Configurable probability to get murdered when sleeping outside your properties
  - Safe zones around Private and Business Properties
  - Optional respawn with hospital bill or standard behavior (loading last save)
- **Arrested Event**: Configurable probability to get arrested when sleeping outside your properties
  - Safe zones around Private and Business Properties
  - Product confiscation and crime charges applied

### Fixed
- Fixed menu loading bug (error when loading into game then back to menu)

### Changed
- Adjusted default configuration accordingly

## [1.0.2]

### Fixed
- Fixed ZIP structure for proper installation

## [1.0.1]

### Added
- Configurable delay mechanism beginning at 4:00 AM before forced sleep activation
- Automatic Sleep Animation Skipping for multiplayer usage
- Automatic continuation through sleep canvas for multiplayer compatibility

## [1.0.0]

### Added
- Initial release with all advertised features
- Forced sleep mechanics
- Positive and negative post-sleep effects
- Configurable effect durations and probabilities
- Visual effect indicators/notifications
- Dynamic phone app integration
- Legit mode for JSON-only configuration
- Multiplayer support

### Notes
- Designed specifically for IL2CPP game version (standard/beta)
- Mono version compatibility not fully tested
