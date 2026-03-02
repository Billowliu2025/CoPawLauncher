"""
将 PNG 图片转换为 ICO 图标文件
包含多种尺寸以支持不同场景
"""
from PIL import Image
import os
import io

def png_to_ico(png_path, ico_path):
    """
    将 PNG 转换为 ICO，包含多个尺寸
    """
    # 打开 PNG 图片
    img = Image.open(png_path)
    
    # 定义需要的尺寸（ICO 支持的标准尺寸）
    sizes = [
        (16, 16),
        (24, 24),
        (32, 32),
        (48, 48),
        (64, 64),
        (128, 128),
        (256, 256)
    ]
    
    # 调整图片到不同尺寸
    ico_images = []
    for size in sizes:
        # 使用 LANCZOS 重采样滤镜获得最佳质量
        resized = img.resize(size, Image.Resampling.LANCZOS)
        # 转换为 RGBA 模式（ICO 需要）
        if resized.mode != 'RGBA':
            resized = resized.convert('RGBA')
        ico_images.append(resized)
    
    # 使用字节流保存
    output = io.BytesIO()
    
    # 保存第一个尺寸
    ico_images[0].save(
        output,
        format='ICO',
        sizes=[(s.width, s.height) for s in ico_images]
    )
    
    # 写入文件
    with open(ico_path, 'wb') as f:
        f.write(output.getvalue())
    
    print(f"[OK] Icon generated: {ico_path}")
    print(f"  Sizes: {', '.join([f'{s[0]}x{s[1]}' for s in sizes])}")

if __name__ == '__main__':
    # 获取脚本所在目录
    script_dir = os.path.dirname(os.path.abspath(__file__))
    png_file = os.path.join(script_dir, 'logo.png')
    ico_file = os.path.join(script_dir, 'app.ico')
    
    if not os.path.exists(png_file):
        print(f"[ERROR] File not found: {png_file}")
        exit(1)
    
    try:
        png_to_ico(png_file, ico_file)
        print("\n[SUCCESS] Conversion completed!")
    except Exception as e:
        print(f"\n[ERROR] Conversion failed: {e}")
        print("\nTip: Please install Pillow library first")
        print("Run: pip install Pillow")
        exit(1)
