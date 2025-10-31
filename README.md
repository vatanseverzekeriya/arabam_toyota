# Stardew Valley Clone

C# ile oyun motoru kullanmadan yapılmış Stardew Valley klonu. Silk.NET (OpenGL wrapper) kullanarak low-level rendering yapar.

## Özellikler

### ✅ Tamamlanan Sistemler

1. **Temel Oyun Altyapısı**
   - Game loop
   - Input yönetimi (klavye ve mouse)
   - OpenGL tabanlı rendering
   - Sahne yönetimi

2. **Zaman Sistemi**
   - Gerçek zamanlı saat sistemi (6:00 - 26:00)
   - Gün, mevsim ve yıl takibi
   - Stardew Valley benzeri zaman akışı

3. **Karakter Sistemi**
   - Karakter hareketi (WASD/Arrow keys)
   - 4 yönlü hareket ve animasyon
   - Enerji sistemi
   - Para sistemi

4. **Farming Sistemi**
   - Toprak işleme (Hoe)
   - Tohum ekimi
   - Sulama sistemi
   - Büyüme mekanizması (gün bazlı)
   - Hasat sistemi
   - Çoklu ürün desteği (Parsnip, Potato, Tomato, vb.)

5. **Inventory Sistemi**
   - 36 slotlu envanter
   - Stackable items
   - Tool ve seed yönetimi
   - Hotbar (1-9 tuşları ile seçim)

6. **Tile-Based Map Sistemi**
   - 50x50 grid yapısı
   - Dinamik tile durumları (Normal, Tilled, Planted)
   - Kamera takip sistemi

### 🚧 Planlanmış Özellikler

- NPC sistemi ve dialog mekanizması
- Evlilik sistemi
- Madencilik sistemi
- Balık tutma
- Crafting sistemi
- Building/upgrade sistemi
- Mevsim bazlı ürün sistemi
- Şehir ve diğer lokasyonlar
- Quest sistemi
- Save/Load sistemi
- Müzik ve ses efektleri

## Gereksinimler

- .NET 8.0 SDK
- OpenGL 3.3+ destekleyen grafik kartı
- Windows/Linux/macOS (cross-platform)

## Kurulum ve Çalıştırma

### 1. .NET SDK Kurulumu

#### Windows
```bash
# Scoop ile (önerilen)
scoop install dotnet-sdk

# veya resmi installer:
# https://dotnet.microsoft.com/download
```

#### Linux
```bash
# Ubuntu/Debian
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0

# Arch Linux
sudo pacman -S dotnet-sdk
```

#### macOS
```bash
brew install dotnet-sdk
```

### 2. Projeyi Çalıştırma

```bash
# Package'ları yükle
dotnet restore

# Projeyi derle
dotnet build

# Oyunu çalıştır
dotnet run
```

## Kontroller

- **WASD veya Arrow Keys**: Karakter hareketi
- **C veya Left Click**: Tool kullanımı
- **1-9**: Inventory slot seçimi
- **ESC**: Çıkış (planlanmış)

## Proje Yapısı

```
StardewValleyClone/
├── Core/
│   └── Game.cs              # Ana oyun sınıfı
├── Engine/
│   ├── TimeManager.cs       # Zaman ve mevsim sistemi
│   ├── InputManager.cs      # Input yönetimi
│   ├── ResourceManager.cs   # Asset yönetimi
│   └── SceneManager.cs      # Sahne yönetimi
├── Graphics/
│   ├── Renderer.cs          # OpenGL renderer
│   ├── Texture.cs           # Texture yükleme ve yönetimi
│   ├── Shader.cs            # Shader yönetimi
│   └── Sprite.cs            # Sprite ve animasyon
├── GameObjects/
│   ├── GameObject.cs        # Base game object
│   ├── Player.cs            # Oyuncu karakteri
│   └── FarmScene.cs         # Farm sahnesi
├── Systems/
│   ├── Inventory.cs         # Envanter sistemi
│   └── FarmingSystem.cs     # Farming mekanikleri
├── Assets/
│   ├── Textures/            # Sprite ve texture'lar
│   ├── Sounds/              # Ses efektleri
│   └── Data/                # JSON data dosyaları
└── Program.cs               # Entry point
```

## Geliştirme Notları

### Farming Sistemi
- Stardew Valley'deki gibi, her ürünün büyüme süresi farklıdır
- Crops her gün sulanmalıdır, aksi takdirde büyümez
- Bazı ürünler multiple yield verir (örn. patates 1-3 adet)

### Zaman Sistemi
- 10 dakika oyun içi = 7 saniye gerçek zaman
- Gün 6:00 AM'de başlar, 2:00 AM'de biter
- Her mevsim 28 gün sürer

### Rendering
- OpenGL 3.3 core profile kullanır
- Pixel art için nearest neighbor filtering
- Tile-based rendering ile optimize edilmiştir

## Asset'ler

Şu anda placeholder texture'lar kullanılmaktadır. Gerçek sprite'ları eklemek için:

1. `Assets/Textures/` klasörüne sprite'ları ekleyin
2. `ResourceManager.cs` içinde texture yükleme kodunu güncelleyin
3. `Player.cs` ve `FarmScene.cs` içindeki placeholder'ları değiştirin

## Lisans

Bu proje, Stardew Valley'in bire bir klonudur ve telif hakları size aittir.

## Katkıda Bulunma

1. Fork yapın
2. Feature branch oluşturun (`git checkout -b feature/amazing-feature`)
3. Commit yapın (`git commit -m 'Add amazing feature'`)
4. Push yapın (`git push origin feature/amazing-feature`)
5. Pull Request açın

## Yapılacaklar (TODO)

- [ ] NPC sistemi ekle
- [ ] Dialog sistemi ekle
- [ ] Madencilik sistemi ekle
- [ ] Balık tutma sistemi ekle
- [ ] Crafting sistemi ekle
- [ ] UI/HUD rendering ekle
- [ ] Save/Load sistemi ekle
- [ ] Müzik ve ses efektleri ekle
- [ ] Daha fazla ürün tipi ekle
- [ ] Hayvan yetiştirme sistemi ekle
- [ ] Mevsimsel değişiklikler ekle
- [ ] Weather sistemi ekle
