import ProtectedRoute from '../../components/ProtectedRoute';

export default function ReportsPage() {
  return (
    <ProtectedRoute>
      <div className="p-6">
        <h1 className="text-2xl font-bold text-gray-900 mb-2">Reports</h1>
        <p className="text-gray-600">Generate and explore reports.</p>
      </div>
    </ProtectedRoute>
  );
}

