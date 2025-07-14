import React, { useEffect, useState } from 'react';
import { ref, getDownloadURL } from 'firebase/storage';
import { storage } from '../config/firebase';

interface Props {
  content: string; // the Firestore URL
}

const ScreenshotRenderer: React.FC<Props> = ({ content }) => {
  const [imgURL, setImgURL] = useState<string | null>(null);

  useEffect(() => {
    const path = extractStoragePath(content);
    if (!path) return;

    const imageRef = ref(storage, path);
    getDownloadURL(imageRef)
      .then((url) => setImgURL(url))
      .catch((err) => {
        console.error('❌ Failed to fetch image URL:', err);
        setImgURL(null);
      });
  }, [content]);

  const extractStoragePath = (url: string) => {
    const base = 'https://storage.googleapis.com/farmerproject-106b6.appspot.com/';
    return url.startsWith(base) ? url.replace(base, '') : '';
  };

  if (!imgURL) return <span className="text-sm text-gray-400">Image not available</span>;

  return (
    <img
      src={imgURL}
      alt="screenshot"
      className="max-h-40 rounded-lg border border-gray-200 shadow"
      onError={() => console.error('❌ Image load failed')}
    />
  );
};

export default ScreenshotRenderer;
