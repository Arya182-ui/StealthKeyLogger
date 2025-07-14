import React, { useState, useEffect } from 'react';
import { collection, onSnapshot, query, orderBy, Timestamp, deleteDoc, doc } from 'firebase/firestore';
import { signOut } from 'firebase/auth';
import { db, auth } from '../config/firebase';
import { KeystrokeLog } from '../types/KeystrokeLog';
import SearchBar from './SearchBar';
import LogsTable from './LogsTable';
import { 
  LogOut, 
  Activity, 
  Globe, 
  Calendar, 
  Wifi, 
  WifiOff, 
  Shield,
  TrendingUp,
  Users,
  Zap,
  Eye,
  Server,
  Trash2
} from 'lucide-react';

const Dashboard: React.FC = () => {
  const [logs, setLogs] = useState<KeystrokeLog[]>([]);
  const [filteredLogs, setFilteredLogs] = useState<KeystrokeLog[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [searchTerm, setSearchTerm] = useState('');
  const [isOnline, setIsOnline] = useState(navigator.onLine);
  const [showBulkDelete, setShowBulkDelete] = useState(false);

  useEffect(() => {
    const handleOnline = () => setIsOnline(true);
    const handleOffline = () => setIsOnline(false);

    window.addEventListener('online', handleOnline);
    window.addEventListener('offline', handleOffline);

    return () => {
      window.removeEventListener('online', handleOnline);
      window.removeEventListener('offline', handleOffline);
    };
  }, []);

  useEffect(() => {
    const q = query(collection(db, 'keystrokes'), orderBy('timestamp', 'desc'));
    
    const unsubscribe = onSnapshot(
      q,
      (querySnapshot) => {
        const logsData: KeystrokeLog[] = [];
        querySnapshot.forEach((doc) => {
          const data = doc.data();
          logsData.push({
            id: doc.id,
            content: data.content || '',
            ip: data.ip || 'Unknown',
            timestamp: data.timestamp instanceof Timestamp 
              ? data.timestamp.toDate() 
              : new Date(data.timestamp) || new Date(),
              type: data.type || 'keystroke',
          });
        });
        setLogs(logsData);
        setLoading(false);
        setError('');
      },
      (err) => {
        console.error('Error fetching logs:', err);
        setError('Failed to fetch logs. Please check your connection and try again.');
        setLoading(false);
      }
    );

    return () => unsubscribe();
  }, []);

  useEffect(() => {
    if (!searchTerm) {
      setFilteredLogs(logs);
    } else {
      const filtered = logs.filter(
        (log) =>
          log.content.toLowerCase().includes(searchTerm.toLowerCase()) ||
          log.ip.toLowerCase().includes(searchTerm.toLowerCase())
      );
      setFilteredLogs(filtered);
    }
  }, [logs, searchTerm]);

  const handleLogout = async () => {
    try {
      await signOut(auth);
    } catch (err) {
      console.error('Error signing out:', err);
    }
  };

  const getStats = () => {
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    
    const todayLogs = logs.filter(log => log.timestamp >= today);
    const uniqueIPs = new Set(logs.map(log => log.ip)).size;
    const lastHour = new Date(Date.now() - 60 * 60 * 1000);
    const recentLogs = logs.filter(log => log.timestamp >= lastHour);
    
    return {
      total: logs.length,
      today: todayLogs.length,
      uniqueIPs,
      lastHour: recentLogs.length
    };
  };

  const handleBulkDelete = async () => {
    if (window.confirm(`Are you sure you want to delete all ${filteredLogs.length} logs? This action cannot be undone.`)) {
      try {
        const deletePromises = filteredLogs.map(log => 
          deleteDoc(doc(db, 'keystrokes', log.id))
        );
        await Promise.all(deletePromises);
      } catch (error) {
        console.error('Error deleting logs:', error);
      }
    }
  };
  const stats = getStats();

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-50 via-purple-50 to-cyan-50">
      {/* Animated Background */}
      <div className="fixed inset-0 overflow-hidden pointer-events-none">
        <div className="absolute -top-40 -right-40 w-80 h-80 bg-purple-300 rounded-full mix-blend-multiply filter blur-xl opacity-20 animate-pulse"></div>
        <div className="absolute -bottom-40 -left-40 w-80 h-80 bg-cyan-300 rounded-full mix-blend-multiply filter blur-xl opacity-20 animate-pulse animation-delay-2000"></div>
      </div>

      {/* Header */}
      <header className="relative backdrop-blur-xl bg-white/80 shadow-xl border-b border-white/20">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between items-center h-20">
            <div className="flex items-center gap-4">
              <div className="relative">
                <div className="bg-gradient-to-r from-purple-600 to-cyan-600 p-3 rounded-2xl shadow-lg">
                  <Shield className="w-8 h-8 text-white" />
                </div>
                <div className="absolute -top-1 -right-1 w-4 h-4 bg-green-400 rounded-full border-2 border-white animate-ping"></div>
              </div>
              <div>
                <h1 className="text-2xl font-bold bg-gradient-to-r from-purple-600 to-cyan-600 bg-clip-text text-transparent">
                  SecureLog Dashboard
                </h1>
                <div className="flex items-center gap-3 text-sm text-gray-600">
                  {isOnline ? (
                    <>
                      <div className="flex items-center gap-1">
                        <Wifi className="w-4 h-4 text-green-500" />
                        <span className="text-green-600 font-medium">Live Monitoring</span>
                      </div>
                    </>
                  ) : (
                    <>
                      <div className="flex items-center gap-1">
                        <WifiOff className="w-4 h-4 text-red-500" />
                        <span className="text-red-600 font-medium">Offline</span>
                      </div>
                    </>
                  )}
                  <div className="w-1 h-1 bg-gray-400 rounded-full"></div>
                  <span>Real-time updates</span>
                </div>
              </div>
            </div>
            <button
              onClick={handleLogout}
              className="flex items-center gap-2 px-6 py-3 text-gray-700 hover:text-gray-900 bg-white/50 hover:bg-white/80 rounded-2xl transition-all duration-300 backdrop-blur-sm border border-white/20 shadow-lg hover:shadow-xl"
            >
              <LogOut className="w-4 h-4" />
              <span className="font-medium">Logout</span>
            </button>
          </div>
        </div>
      </header>

      <main className="relative max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Stats Cards */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
          <div className="group relative bg-white/80 backdrop-blur-xl rounded-3xl shadow-xl border border-white/20 p-6 hover:shadow-2xl transition-all duration-300 hover:-translate-y-1">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-600 mb-1">Total Logs</p>
                <p className="text-3xl font-bold bg-gradient-to-r from-purple-600 to-purple-800 bg-clip-text text-transparent">
                  {stats.total.toLocaleString()}
                </p>
                <p className="text-xs text-gray-500 mt-1">All time</p>
              </div>
              <div className="bg-gradient-to-r from-purple-500 to-purple-600 p-4 rounded-2xl shadow-lg group-hover:scale-110 transition-transform">
                <Activity className="w-6 h-6 text-white" />
              </div>
            </div>
            <div className="absolute inset-0 rounded-3xl bg-gradient-to-r from-purple-500/10 to-purple-600/10 opacity-0 group-hover:opacity-100 transition-opacity duration-300"></div>
          </div>
          
          <div className="group relative bg-white/80 backdrop-blur-xl rounded-3xl shadow-xl border border-white/20 p-6 hover:shadow-2xl transition-all duration-300 hover:-translate-y-1">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-600 mb-1">Unique Sources</p>
                <p className="text-3xl font-bold bg-gradient-to-r from-cyan-600 to-cyan-800 bg-clip-text text-transparent">
                  {stats.uniqueIPs}
                </p>
                <p className="text-xs text-gray-500 mt-1">IP addresses</p>
              </div>
              <div className="bg-gradient-to-r from-cyan-500 to-cyan-600 p-4 rounded-2xl shadow-lg group-hover:scale-110 transition-transform">
                <Globe className="w-6 h-6 text-white" />
              </div>
            </div>
            <div className="absolute inset-0 rounded-3xl bg-gradient-to-r from-cyan-500/10 to-cyan-600/10 opacity-0 group-hover:opacity-100 transition-opacity duration-300"></div>
          </div>
          
          <div className="group relative bg-white/80 backdrop-blur-xl rounded-3xl shadow-xl border border-white/20 p-6 hover:shadow-2xl transition-all duration-300 hover:-translate-y-1">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-600 mb-1">Today's Activity</p>
                <p className="text-3xl font-bold bg-gradient-to-r from-green-600 to-green-800 bg-clip-text text-transparent">
                  {stats.today.toLocaleString()}
                </p>
                <p className="text-xs text-gray-500 mt-1">Last 24 hours</p>
              </div>
              <div className="bg-gradient-to-r from-green-500 to-green-600 p-4 rounded-2xl shadow-lg group-hover:scale-110 transition-transform">
                <Calendar className="w-6 h-6 text-white" />
              </div>
            </div>
            <div className="absolute inset-0 rounded-3xl bg-gradient-to-r from-green-500/10 to-green-600/10 opacity-0 group-hover:opacity-100 transition-opacity duration-300"></div>
          </div>

          <div className="group relative bg-white/80 backdrop-blur-xl rounded-3xl shadow-xl border border-white/20 p-6 hover:shadow-2xl transition-all duration-300 hover:-translate-y-1">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-600 mb-1">Last Hour</p>
                <p className="text-3xl font-bold bg-gradient-to-r from-orange-600 to-orange-800 bg-clip-text text-transparent">
                  {stats.lastHour.toLocaleString()}
                </p>
                <p className="text-xs text-gray-500 mt-1">Recent activity</p>
              </div>
              <div className="bg-gradient-to-r from-orange-500 to-orange-600 p-4 rounded-2xl shadow-lg group-hover:scale-110 transition-transform">
                <Zap className="w-6 h-6 text-white" />
              </div>
            </div>
            <div className="absolute inset-0 rounded-3xl bg-gradient-to-r from-orange-500/10 to-orange-600/10 opacity-0 group-hover:opacity-100 transition-opacity duration-300"></div>
          </div>
        </div>

        {/* Search Bar */}
        <div className="mb-8">
          <div className="flex gap-4 items-center">
            <div className="flex-1">
              <SearchBar searchTerm={searchTerm} onSearchChange={setSearchTerm} />
            </div>
            {filteredLogs.length > 0 && (
              <button
                onClick={handleBulkDelete}
                className="flex items-center gap-2 px-6 py-4 bg-red-50 hover:bg-red-100 text-red-600 hover:text-red-700 rounded-2xl transition-all duration-300 border border-red-200 hover:border-red-300 shadow-lg hover:shadow-xl backdrop-blur-sm"
              >
                <Trash2 className="w-5 h-5" />
                <span className="font-medium">Clear All ({filteredLogs.length})</span>
              </button>
            )}
          </div>
        </div>

        {/* Error Message */}
        {error && (
          <div className="bg-red-500/20 backdrop-blur-sm border border-red-500/30 text-red-700 px-6 py-4 rounded-2xl mb-8 shadow-lg">
            <div className="flex items-center gap-3">
              <div className="w-2 h-2 bg-red-500 rounded-full animate-pulse"></div>
              <span className="font-medium">{error}</span>
            </div>
          </div>
        )}

        {/* Logs Table */}
        <LogsTable logs={filteredLogs} loading={loading} />
      </main>
    </div>
  );
};

export default Dashboard;