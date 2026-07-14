#!/usr/bin/env python3
"""Convert RSC .ob3 model files to Wavefront .obj format with decoded PNG textures.

Usage: python3 convert_models.py

Output:
  OpenRS/Data/Models/textures/  -- decoded 128x128 RGBA PNG texture files
  OpenRS/Data/Models/obj/       -- .obj and .mtl files for each model
"""

import json
import struct
import zlib
from pathlib import Path

SCRIPT_DIR = Path(__file__).parent
DATA_DIR = SCRIPT_DIR / 'OpenRS' / 'Data'
TEXTURES_DIR = DATA_DIR / 'Textures'
MODELS_DIR = DATA_DIR / 'Models'
OUTPUT_TEXTURES_DIR = MODELS_DIR / 'textures'
OUTPUT_OBJ_DIR = MODELS_DIR / 'obj'

SHADE_SENTINEL = 32767
SHADE_RGB = (188, 97, 78)  # 0xbc614e - face shade colour

# Any texture index < 0 means the face side is not rendered (single-sided culling).
INVISIBLE_FACE_SIDE = -1


# --- Minimal RGBA PNG writer (no external dependencies) ---

def _png_chunk(chunk_type: bytes, data: bytes) -> bytes:
    length = struct.pack('>I', len(data))
    crc = struct.pack('>I', zlib.crc32(chunk_type + data) & 0xffffffff)
    return length + chunk_type + data + crc


def write_rgba_png(
    pixels_rgba: list[tuple[int, int, int, int]], width: int, height: int, path: Path
) -> None:
    """Write an RGBA PNG using only Python standard-library modules."""
    signature = b'\x89PNG\r\n\x1a\n'

    ihdr_data = struct.pack('>IIBBBBB', width, height, 8, 6, 0, 0, 0)
    ihdr = _png_chunk(b'IHDR', ihdr_data)

    raw_lines = bytearray()
    for row in range(height):
        raw_lines.append(0)  # filter byte: None
        for col in range(width):
            r, g, b, a = pixels_rgba[row * width + col]
            raw_lines += bytes([r, g, b, a])

    idat = _png_chunk(b'IDAT', zlib.compress(bytes(raw_lines), 9))
    iend = _png_chunk(b'IEND', b'')

    path.write_bytes(signature + ihdr + idat + iend)


# --- RSC palette-indexed image decoder ---

def decode_image(image_data: bytes, meta_data: bytes) -> tuple[list[int], int, int]:
    """Decode one RSC indexed-colour image.

    image_data: raw .dat file bytes.
    meta_data: raw index.dat bytes.
    Returns (rgb_pixels, assumed_width, assumed_height) where rgb_pixels is a flat
    list of packed 0xRRGGBB integers; 0xff00ff denotes transparent (palette index 0).
    """
    meta_offset = (image_data[0] << 8) | image_data[1]

    pos = meta_offset
    assumed_width = (meta_data[pos] << 8) | meta_data[pos + 1]
    assumed_height = (meta_data[pos + 2] << 8) | meta_data[pos + 3]
    colour_count = meta_data[pos + 4]
    pos += 5

    # palette[0] = transparent magenta (implicit)
    palette = [0xff00ff]
    for _ in range(colour_count - 1):
        red = meta_data[pos]
        green = meta_data[pos + 1]
        blue = meta_data[pos + 2]
        palette.append((red << 16) | (green << 8) | blue)
        pos += 3

    # Per-image header: offsetX, offsetY, width, height, layout flag
    offset_x = meta_data[pos]; pos += 1
    offset_y = meta_data[pos]; pos += 1
    img_width = (meta_data[pos] << 8) | meta_data[pos + 1]; pos += 2
    img_height = (meta_data[pos] << 8) | meta_data[pos + 1]; pos += 2
    layout = meta_data[pos]

    # Pixel data starts after the 2-byte metadata pointer in image_data
    pixel_indices = image_data[2:]
    pixels = [0xff00ff] * (assumed_width * assumed_height)

    if layout == 0:  # row-major
        src = 0
        for row in range(img_height):
            for col in range(img_width):
                dest = (row + offset_y) * assumed_width + (col + offset_x)
                pixels[dest] = palette[pixel_indices[src] & 0xff]
                src += 1
    else:  # column-major (layout == 1)
        src = 0
        for col in range(img_width):
            for row in range(img_height):
                dest = (row + offset_y) * assumed_width + (col + offset_x)
                pixels[dest] = palette[pixel_indices[src] & 0xff]
                src += 1

    return pixels, assumed_width, assumed_height


def build_composite_texture(meta_data: bytes, main_name: str, sub_name: str) -> tuple[list[int], int, int]:
    """Decode and composite main + optional sub-texture as the game does.

    Replicates LoadTextures(): draw main on magenta background, then overlay sub-texture
    (palette index 0 = transparent) if subName is non-empty.
    Returns (rgb_pixels, width, height).
    """
    main_bytes = (TEXTURES_DIR / (main_name.lower() + '.dat')).read_bytes()
    rgb_pixels, width, height = decode_image(main_bytes, meta_data)

    if sub_name:
        sub_bytes = (TEXTURES_DIR / (sub_name.lower() + '.dat')).read_bytes()
        sub_pixels, _, _ = decode_image(sub_bytes, meta_data)
        for i in range(width * height):
            if sub_pixels[i] != 0xff00ff:  # not transparent
                rgb_pixels[i] = sub_pixels[i]

    # The game replaces pure green (0x00ff00 = 65280) with magenta post-compositing
    for i in range(width * height):
        if rgb_pixels[i] == 0x00ff00:
            rgb_pixels[i] = 0xff00ff

    return rgb_pixels, width, height


def rgb_pixels_to_rgba(rgb_pixels: list[int]) -> list[tuple[int, int, int, int]]:
    """Convert packed 0xRRGGBB list to RGBA tuples; magenta (0xff00ff) → alpha 0."""
    result = []
    for pixel in rgb_pixels:
        red = (pixel >> 16) & 0xff
        green = (pixel >> 8) & 0xff
        blue = pixel & 0xff
        alpha = 0 if (red == 0xff and green == 0x00 and blue == 0xff) else 255
        result.append((red, green, blue, alpha))
    return result


# --- Texture decoding pipeline ---

def decode_all_textures() -> dict[int, str]:
    """Decode all textures from textures.json to PNG files in OUTPUT_TEXTURES_DIR.

    Returns a mapping of texture_id (int) -> PNG filename (relative to OUTPUT_TEXTURES_DIR).
    """
    meta_data = (TEXTURES_DIR / 'index.dat').read_bytes()
    textures_entries = json.loads((DATA_DIR / 'textures.json').read_text())

    OUTPUT_TEXTURES_DIR.mkdir(parents=True, exist_ok=True)

    texture_id_to_png: dict[int, str] = {}

    for entry in textures_entries:
        texture_id = int(entry['id'])
        name = entry['name']
        sub_name = entry.get('subName', '')

        png_name = f'texture_{texture_id:02d}_{name.lower()}'
        if sub_name:
            png_name += f'_{sub_name.lower()}'
        png_name += '.png'

        png_path = OUTPUT_TEXTURES_DIR / png_name

        if not png_path.exists():
            try:
                rgb_pixels, width, height = build_composite_texture(meta_data, name, sub_name)
                rgba_pixels = rgb_pixels_to_rgba(rgb_pixels)
                write_rgba_png(rgba_pixels, width, height, png_path)
                print(f'  Decoded texture {texture_id:02d}: {png_name}')
            except FileNotFoundError as error:
                print(f'  Warning: missing file for texture {texture_id} ({name}): {error}')
                continue

        texture_id_to_png[texture_id] = png_name

    return texture_id_to_png


# --- .ob3 parser ---

def parse_ob3(data: bytes) -> dict:
    """Parse an RSC .ob3 model file into a dictionary of geometry data."""
    offset = 0

    vert_count = struct.unpack_from('>H', data, offset)[0]; offset += 2
    face_count = struct.unpack_from('>H', data, offset)[0]; offset += 2

    if vert_count == 0 or face_count == 0:
        return {'verts': [], 'faces': [], 'texture_back': [], 'texture_front': [], 'gouraud_shade': []}

    verts_x = list(struct.unpack_from(f'>{vert_count}h', data, offset)); offset += vert_count * 2
    verts_y = list(struct.unpack_from(f'>{vert_count}h', data, offset)); offset += vert_count * 2
    verts_z = list(struct.unpack_from(f'>{vert_count}h', data, offset)); offset += vert_count * 2

    face_vert_counts = list(struct.unpack_from(f'>{face_count}B', data, offset)); offset += face_count

    texture_back = list(struct.unpack_from(f'>{face_count}h', data, offset)); offset += face_count * 2
    texture_front = list(struct.unpack_from(f'>{face_count}h', data, offset)); offset += face_count * 2
    gouraud_shade = list(struct.unpack_from(f'>{face_count}B', data, offset)); offset += face_count

    # Vertex indices are unsigned: 1 byte if vert_count < 256, else 2 bytes (big-endian)
    index_format = '>H' if vert_count >= 256 else '>B'
    index_size = 2 if vert_count >= 256 else 1

    face_vertices = []
    for count in face_vert_counts:
        indices = []
        for _ in range(count):
            index = struct.unpack_from(index_format, data, offset)[0]
            offset += index_size
            indices.append(index)
        face_vertices.append(indices)

    verts = list(zip(verts_x, verts_y, verts_z))
    return {
        'verts': verts,
        'faces': face_vertices,
        'texture_back': texture_back,
        'texture_front': texture_front,
        'gouraud_shade': gouraud_shade,
    }


# --- Per-face planar UV projection ---

def compute_face_uvs(
    verts: list[tuple[int, int, int]], face_indices: list[int]
) -> list[tuple[float, float]]:
    """Compute planar UV coordinates for a face, normalised to fill [0, 1] x [0, 1].

    Projects each vertex onto the 2D plane perpendicular to the dominant face normal
    axis, then normalises the result to [0, 1] across the face bounding box.
    This replicates the RSC renderer's behaviour of mapping the full texture onto
    each polygon.
    """
    if len(face_indices) < 2:
        return [(0.0, 0.0)] * len(face_indices)

    v0 = verts[face_indices[0]]
    v1 = verts[face_indices[1]]
    v2 = verts[face_indices[min(2, len(face_indices) - 1)]]

    edge1 = (v1[0] - v0[0], v1[1] - v0[1], v1[2] - v0[2])
    edge2 = (v2[0] - v0[0], v2[1] - v0[1], v2[2] - v0[2])

    normal_x = edge1[1] * edge2[2] - edge1[2] * edge2[1]
    normal_y = edge1[2] * edge2[0] - edge1[0] * edge2[2]
    normal_z = edge1[0] * edge2[1] - edge1[1] * edge2[0]

    abs_x = abs(normal_x)
    abs_y = abs(normal_y)
    abs_z = abs(normal_z)

    # Project onto the plane perpendicular to the dominant normal axis
    if abs_y >= abs_x and abs_y >= abs_z:
        projected = [(verts[i][0], verts[i][2]) for i in face_indices]
    elif abs_x >= abs_y and abs_x >= abs_z:
        projected = [(verts[i][1], verts[i][2]) for i in face_indices]
    else:
        projected = [(verts[i][0], verts[i][1]) for i in face_indices]

    us = [p[0] for p in projected]
    vs = [p[1] for p in projected]
    u_min, u_max = min(us), max(us)
    v_min, v_max = min(vs), max(vs)
    u_range = float(u_max - u_min) if u_max != u_min else 1.0
    v_range = float(v_max - v_min) if v_max != v_min else 1.0

    return [((u - u_min) / u_range, (v - v_min) / v_range) for u, v in projected]


# --- Material name resolution ---

def material_name_for(texture_id: int, texture_id_to_png: dict[int, str]) -> str:
    """Resolve a texture index to an MTL material name."""
    if texture_id == SHADE_SENTINEL:
        return 'mat_shade'
    if texture_id < 0 or texture_id not in texture_id_to_png:
        return 'mat_default'
    return f'mat_tex_{texture_id:02d}'


# --- .mtl writer ---

def write_mtl_file(
    mtl_path: Path, used_materials: set[str], texture_id_to_png: dict[int, str]
) -> None:
    lines = ['# Generated by convert_models.py\n\n']

    if 'mat_shade' in used_materials:
        red, green, blue = SHADE_RGB
        lines.append('newmtl mat_shade\n')
        lines.append(f'Kd {red / 255:.4f} {green / 255:.4f} {blue / 255:.4f}\n')
        lines.append('Ka 0.0000 0.0000 0.0000\n')
        lines.append('Ks 0.0000 0.0000 0.0000\n\n')

    if 'mat_default' in used_materials:
        lines.append('newmtl mat_default\n')
        lines.append('Kd 0.8000 0.8000 0.8000\n')
        lines.append('Ka 0.0000 0.0000 0.0000\n')
        lines.append('Ks 0.0000 0.0000 0.0000\n\n')

    for material_name in sorted(used_materials):
        if not material_name.startswith('mat_tex_'):
            continue
        texture_id = int(material_name[len('mat_tex_'):])
        if texture_id not in texture_id_to_png:
            continue
        png_name = texture_id_to_png[texture_id]
        lines.append(f'newmtl {material_name}\n')
        lines.append(f'map_Kd ../textures/{png_name}\n')
        lines.append('Ka 0.0000 0.0000 0.0000\n')
        lines.append('Ks 0.0000 0.0000 0.0000\n\n')

    mtl_path.write_text(''.join(lines))


# --- .obj writer ---

def convert_ob3_to_obj(
    ob3_path: Path, output_dir: Path, texture_id_to_png: dict[int, str]
) -> None:
    model_name = ob3_path.stem
    model = parse_ob3(ob3_path.read_bytes())

    if not model['verts']:
        return  # Skip empty models

    verts = model['verts']
    faces = model['faces']
    texture_back = model['texture_back']
    texture_front = model['texture_front']

    mtl_filename = model_name + '.mtl'
    obj_lines = [f'# Converted from {ob3_path.name}\n', f'mtllib {mtl_filename}\n\n']

    # RSC Y-axis increases downward; negate to follow standard 3D convention (Y up).
    for x, y, z in verts:
        obj_lines.append(f'v {x} {-y} {z}\n')

    obj_lines.append('\n')

    uv_entries: list[tuple[float, float]] = []
    face_records: list[tuple[str, list[tuple[int, int]]]] = []

    def record_face(face_indices: list[int], texture_id: int, reverse_winding: bool) -> None:
        material = material_name_for(texture_id, texture_id_to_png)
        face_uvs = compute_face_uvs(verts, face_indices)

        uv_base = len(uv_entries) + 1  # 1-based OBJ index

        if reverse_winding:
            ordered_indices = list(reversed(face_indices))
            ordered_uvs = [uv_base + len(face_uvs) - 1 - k for k in range(len(face_uvs))]
        else:
            ordered_indices = face_indices
            ordered_uvs = list(range(uv_base, uv_base + len(face_uvs)))

        for uv in face_uvs:
            uv_entries.append(uv)

        vertex_uv_pairs = [
            (ordered_indices[k] + 1, ordered_uvs[k])
            for k in range(len(ordered_indices))
        ]
        face_records.append((material, vertex_uv_pairs))

    for face_index, face_vert_indices in enumerate(faces):
        if len(face_vert_indices) < 3:
            continue

        front_texture = texture_front[face_index]
        back_texture = texture_back[face_index]

        if front_texture != INVISIBLE_FACE_SIDE:
            record_face(face_vert_indices, front_texture, reverse_winding=False)

        if back_texture != INVISIBLE_FACE_SIDE and back_texture != front_texture:
            record_face(face_vert_indices, back_texture, reverse_winding=True)

    for u, v in uv_entries:
        obj_lines.append(f'vt {u:.6f} {v:.6f}\n')

    obj_lines.append('\n')

    used_materials: set[str] = set()
    current_material = ''

    for material, vertex_uv_pairs in face_records:
        if material != current_material:
            obj_lines.append(f'usemtl {material}\n')
            current_material = material
        used_materials.add(material)
        tokens = ' '.join(f'{vertex}/{uv}' for vertex, uv in vertex_uv_pairs)
        obj_lines.append(f'f {tokens}\n')

    (output_dir / (model_name + '.obj')).write_text(''.join(obj_lines))
    write_mtl_file(output_dir / mtl_filename, used_materials, texture_id_to_png)


# --- Entry point ---

def main() -> None:
    print('Step 1: Decoding textures...')
    texture_id_to_png = decode_all_textures()
    print(f'  {len(texture_id_to_png)} textures decoded to {OUTPUT_TEXTURES_DIR}')

    OUTPUT_OBJ_DIR.mkdir(parents=True, exist_ok=True)

    ob3_files = sorted(MODELS_DIR.glob('*.ob3'))
    print(f'\nStep 2: Converting {len(ob3_files)} .ob3 files...')

    success_count = 0
    error_count = 0

    for ob3_path in ob3_files:
        try:
            convert_ob3_to_obj(ob3_path, OUTPUT_OBJ_DIR, texture_id_to_png)
            success_count += 1
        except Exception as error:
            print(f'  Error converting {ob3_path.name}: {error}')
            error_count += 1

    print(f'\nDone: {success_count} converted, {error_count} errors.')
    print(f'  Textures: {OUTPUT_TEXTURES_DIR}')
    print(f'  Models:   {OUTPUT_OBJ_DIR}')


if __name__ == '__main__':
    main()
