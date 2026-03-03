"""
将 PNG 图片转换为标准 ICO 图标文件（包含多尺寸）
用法：在项目根目录下运行 python Scripts/convert_icon.py
"""
from PIL import Image
import os
import io


def png_to_ico(png_path: str, ico_path: str) -> None:
    """
    将 PNG 转换为多尺寸 ICO 文件
    :param png_path: 源 PNG 文件路径
    :param ico_path: 输出 ICO 文件路径
    """
    sizes = [
        (16, 16),
        (24, 24),
        (32, 32),
        (48, 48),
        (64, 64),
        (128, 128),
        (256, 256),
    ]

    with Image.open(png_path) as img:
        if img.mode != 'RGBA':
            img = img.convert('RGBA')

        frames = [img.resize(s, Image.Resampling.LANCZOS) for s in sizes]

    # 以最大帧为基础，附加其余帧
    frames[-1].save(
        ico_path,
        format='ICO',
        append_images=frames[:-1],
        sizes=[(f.width, f.height) for f in frames[::-1]],
    )

    print(f"[OK] 已生成：{ico_path}")
    print(f"     尺寸：{', '.join(f'{s[0]}x{s[1]}' for s in sizes)}")


if __name__ == '__main__':
    # 脚本位于 Scripts/，资源位于 ../Assets/
    script_dir = os.path.dirname(os.path.abspath(__file__))
    assets_dir = os.path.join(script_dir, '..', 'Assets')

    # 源 PNG → 目标 ICO
    png_file = os.path.join(assets_dir, 'Copaw.png')
    ico_file = os.path.join(assets_dir, 'favicon.ico')

    if not os.path.exists(png_file):
        print(f"[ERROR] 源文件不存在：{png_file}")
        raise SystemExit(1)

    try:
        png_to_ico(png_file, ico_file)
        print("\n[SUCCESS] 图标转换完成！")
    except ImportError:
        print("\n[ERROR] 请先安装 Pillow：pip install Pillow")
        raise SystemExit(1)
    except Exception as e:
        print(f"\n[ERROR] 转换失败：{e}")
        raise SystemExit(1)
