import React from "react";
import {
  BrowserRouter as Router,
  Route,
  Routes,
  Navigate,
} from "react-router-dom";
import { Rooms, Login } from "../pages";
import ProtectedRoute from "../routes/ProtectedRoute";
import PublicRoute from "../routes/PublicRoute";
import { Layout } from "../layouts";
function App() {
  return (
    <>
      <React.StrictMode>
        <Router>
          <Routes>
            <Route
              path="/login"
              element={
                <PublicRoute>
                  {" "}
                  <Login />{" "}
                </PublicRoute>
              }
            />
            <Route
              path="/rooms"
              element={
                <Layout>
                  {" "}
                  <ProtectedRoute>
                    {" "}
                    <Rooms />{" "}
                  </ProtectedRoute>{" "}
                </Layout>
              }
            />
            <Route path="*" element={<Navigate to="/rooms" replace />} />
          </Routes>
        </Router>
      </React.StrictMode>
    </>
  );
}

export default App;
