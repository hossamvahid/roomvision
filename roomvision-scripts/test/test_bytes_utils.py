"""Unit tests for bytes_utils."""
import sys
from unittest.mock import MagicMock, patch
import pytest

# conftest.py patches cv2 and numpy before import
import cv2 as cv2_mock
import numpy as np_mock
from utilities.bytes_utils import bytes_to_image


class TestBytesToImage:

    def test_decodes_bytes_and_returns_image(self):
        fake_array = MagicMock()
        fake_image = MagicMock()
        np_mock.frombuffer.return_value = fake_array
        cv2_mock.imdecode.return_value = fake_image
        cv2_mock.IMREAD_COLOR = 1

        result = bytes_to_image(b"\xff\xd8\xff")

        np_mock.frombuffer.assert_called_once_with(b"\xff\xd8\xff", np_mock.uint8)
        cv2_mock.imdecode.assert_called_once_with(fake_array, cv2_mock.IMREAD_COLOR)
        assert result == fake_image

    def test_uses_imread_color_flag(self):
        cv2_mock.IMREAD_COLOR = 42
        np_mock.frombuffer.return_value = MagicMock()

        bytes_to_image(b"\x00")

        _, flags = cv2_mock.imdecode.call_args.args
        assert flags == 42

    def test_passes_raw_bytes_to_frombuffer(self):
        raw = b"\x01\x02\x03\x04"
        np_mock.frombuffer.return_value = MagicMock()

        bytes_to_image(raw)

        np_mock.frombuffer.assert_called_with(raw, np_mock.uint8)
