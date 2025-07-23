# Hệ thống Event và Asset Management

## Tổng quan

Hệ thống này cho phép game tự động thay đổi giao diện và assets dựa trên các sự kiện theo mùa (Winter, Halloween, Christmas, Summer). Các assets được tải động từ S3 và được quản lý thông minh để tiết kiệm bộ nhớ thiết bị.

## Các thành phần chính

### 1. EventManager
- Quản lý trạng thái event hiện tại
- Hỗ trợ nhiều chế độ: Manual, Server-based, Time-based, Hybrid
- Tự động xóa assets khi event kết thúc

### 2. DynamicBackgroundManager
- Thay đổi background dựa trên event hiện tại
- Cache backgrounds để tải nhanh hơn
- Hỗ trợ transition mượt mà

### 3. S3AssetManager
- Quản lý việc tải và cache assets từ S3
- Hỗ trợ sprites, audio, và các loại assets khác
- Tự động dọn dẹp cache để tiết kiệm bộ nhớ

## Cách hoạt động

### Khi game khởi động:
1. EventManager kiểm tra event hiện tại (từ server hoặc thời gian)
2. Nếu event đã thay đổi, xóa assets của event cũ
3. Tải assets của event mới từ S3 (nếu cần)

### Khi event thay đổi:
1. EventManager phát hiện sự thay đổi
2. Xóa ngay lập tức assets của event cũ
3. Tải assets mới cho event hiện tại

## Cấu hình

### EventManager:
```csharp
[Header("Cache Settings")]
[SerializeField] private bool cleanupExpiredEventAssets = true;
[SerializeField] private bool immediateCleanupWhenEventEnds = true; // Xóa ngay khi event kết thúc
```

### S3AssetManager:
```csharp
[Header("Cache Settings")]
[SerializeField] private bool autoCacheCleanup = true;
[SerializeField] private bool cleanupUnusedEvents = true; // Xóa assets của các event không còn active
[SerializeField] private int maxCacheSizeMB = 100;
```

## Cấu trúc S3 Bucket

```
aetheric-game-assets/
├── config/
│   └── events.json
├── backgrounds/
│   ├── winter_main_menu.jpg
│   ├── halloween_main_menu.jpg
│   ├── christmas_main_menu.jpg
│   └── summer_main_menu.jpg
├── sprites/
│   └── tower_skins/
│       ├── fire_winter.png
│       ├── ice_winter.png
│       └── ...
└── audio/
    ├── music/
    │   └── winter_theme.mp3
    └── sfx/
        └── ...
```

## File Config (events.json)

```json
{
    "currentEvent": "winter",
    "eventStartDate": "2024-01-15",
    "eventEndDate": "2024-03-15",
    "eventMessage": "Winter Event is Live! Enjoy special winter towers and backgrounds!",
    "forceUpdate": false,
    "eventAssets": [
        "/backgrounds/winter_main_menu.jpg",
        "/sprites/tower_skins/fire_winter.png",
        "/sprites/tower_skins/ice_winter.png",
        "/audio/music/winter_theme.mp3"
    ]
}
```

## Sử dụng trong code

```csharp
// Kiểm tra event hiện tại
string currentEvent = EventManager.Instance.GetCurrentEvent();

// Kiểm tra có phải winter event không
if (EventManager.Instance.IsEventActive("winter"))
{
    // Do winter stuff
}

// Load sprite từ S3
S3AssetManager.Instance.LoadSprite("/sprites/tower_skins/fire_winter.png", (sprite) => {
    if (sprite != null)
    {
        towerRenderer.sprite = sprite;
    }
});
```

## Các chức năng debug

### EventManager:
- `Force Reload Config`: Tải lại config từ server
- `Activate Winter Event`: Kích hoạt Winter event (development mode)
- `Force Cleanup All Event Assets`: Xóa tất cả assets của tất cả events

### DynamicBackgroundManager:
- `Force Reload Background`: Tải lại background hiện tại
- `Clear Current Background Cache`: Xóa cache của background hiện tại
- `Clear All Background Cache`: Xóa cache của tất cả backgrounds

### S3AssetManager:
- `Clear Memory Cache`: Xóa cache trong bộ nhớ
- `Clear Disk Cache`: Xóa tất cả cache trên disk