import ProtectedRoute from '../../components/ProtectedRoute';

export default function CalendarPage() {
  return (
    <ProtectedRoute>
      <div className="p-6">
        <h1 className="text-2xl font-bold text-gray-900 mb-2">Calendar</h1>
        <p className="text-gray-600">View events and deadlines.</p>
      </div>
    </ProtectedRoute>
  );
}

