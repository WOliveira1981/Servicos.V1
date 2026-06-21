import { useEffect, useMemo, useState } from 'react'
import './App.css'

type Servico = {
  id: string
  nome: string
  descricao: string
  valor: number
  ativo: boolean
}

type Orcamento = {
  id: string
  servicoId: string
  valorTotal: number
  dataCriacao: string
}

type LoginResponse = {
  jwtToken: string
  provider: string
  email?: string
}

const API_BASE = import.meta.env.VITE_API_BASE_URL || '/api'

const sampleMetrics = [42, 56, 38, 74, 91, 64]

function App() {
  const [token, setToken] = useState<string>(localStorage.getItem('token') || '')
  const [email, setEmail] = useState('user@example.com')
  const [name, setName] = useState('Usuário Demo')
  const [message, setMessage] = useState('')
  const [servicos, setServicos] = useState<Servico[]>([])
  const [orcamentos, setOrcamentos] = useState<Orcamento[]>([])
  const [isLoading, setIsLoading] = useState(false)
  const [calculator, setCalculator] = useState('')

  const totalValue = useMemo(
    () => servicos.reduce((sum, servico) => sum + servico.valor, 0),
    [servicos],
  )

  const totalBudgets = useMemo(
    () => orcamentos.reduce((sum, item) => sum + item.valorTotal, 0),
    [orcamentos],
  )

  useEffect(() => {
    if (!token) return

    const fetchData = async () => {
      setIsLoading(true)
      try {
        const [servicosResponse, orcamentosResponse] = await Promise.all([
          fetch(`${API_BASE}/gateway/servicos`, {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }),
          fetch(`${API_BASE}/gateway/servicos`, {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }),
        ])

        if (servicosResponse.ok) {
          const payload = await servicosResponse.json()
          setServicos(payload)
        }

        if (orcamentosResponse.ok) {
          const payload = await orcamentosResponse.json()
          setOrcamentos(payload)
        }
      } catch (error) {
        setMessage('Não foi possível carregar os dados da API.')
      } finally {
        setIsLoading(false)
      }
    }

    fetchData()
  }, [token])

  const handleLogin = async () => {
    setIsLoading(true)
    setMessage('')

    try {
      const response = await fetch(`${API_BASE}/gateway/auth/login`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          provider: 'Google',
          token: 'demo-google-token',
          email,
          name,
        }),
      })

      const payload = await response.json()

      if (!response.ok || !payload.jwtToken) {
        throw new Error(payload.message || 'Falha ao autenticar')
      }

      const authPayload = payload as LoginResponse
      localStorage.setItem('token', authPayload.jwtToken)
      setToken(authPayload.jwtToken)
      setMessage(`Bem-vindo(a), ${authPayload.email || email}`)
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro inesperado')
    } finally {
      setIsLoading(false)
    }
  }

  const handleCalculatorUse = (value: string) => {
    if (value === '=') {
      try {
        setCalculator(String(eval(calculator)))
      } catch {
        setCalculator('Erro')
      }
      return
    }

    if (value === 'C') {
      setCalculator('')
      return
    }

    setCalculator((prev) => `${prev}${value}`)
  }

  const dateLabel = new Date().toLocaleDateString('pt-BR', {
    weekday: 'long',
    day: 'numeric',
    month: 'long',
  })

  return (
    <main className="app-shell">
      <header className="top-bar">
        <div>
          <p className="eyebrow">Painel administrativo</p>
          <h1>Serviços & Orçamentos</h1>
        </div>
        <section className="login-panel" aria-label="Login do usuário">
          <label>
            E-mail
            <input value={email} onChange={(e) => setEmail(e.target.value)} />
          </label>
          <label>
            Nome
            <input value={name} onChange={(e) => setName(e.target.value)} />
          </label>
          <button onClick={handleLogin} disabled={isLoading}>
            {token ? 'Reautenticar' : 'Entrar'}
          </button>
        </section>
      </header>

      {message && <p className="feedback">{message}</p>}

      <section className="stats-grid">
        <article className="stat-card">
          <span>Serviços ativos</span>
          <strong>{servicos.length}</strong>
        </article>
        <article className="stat-card">
          <span>Valor total</span>
          <strong>{totalValue.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}</strong>
        </article>
        <article className="stat-card">
          <span>Orçamentos</span>
          <strong>{orcamentos.length}</strong>
        </article>
        <article className="stat-card">
          <span>Volume mensal</span>
          <strong>{totalBudgets.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}</strong>
        </article>
      </section>

      <section className="content-grid">
        <div className="panel">
          <h2>Visão geral</h2>
          <div className="accordion-list">
            <details open>
              <summary>Serviços disponíveis</summary>
              <ul>
                {servicos.length === 0 ? (
                  <li>Nenhum serviço carregado ainda.</li>
                ) : (
                  servicos.map((servico) => (
                    <li key={servico.id}>
                      <strong>{servico.nome}</strong>
                      <span>{servico.valor.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}</span>
                    </li>
                  ))
                )}
              </ul>
            </details>
            <details>
              <summary>Últimos orçamentos</summary>
              <ul>
                {orcamentos.length === 0 ? (
                  <li>Nenhum orçamento registrado.</li>
                ) : (
                  orcamentos.map((orcamento) => (
                    <li key={orcamento.id}>
                      <strong>{new Date(orcamento.dataCriacao).toLocaleDateString('pt-BR')}</strong>
                      <span>{orcamento.valorTotal.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}</span>
                    </li>
                  ))
                )}
              </ul>
            </details>
          </div>
        </div>

        <div className="panel chart-panel">
          <h2>Histograma</h2>
          <div className="histogram" role="img" aria-label="Histograma de desempenho">
            {sampleMetrics.map((value, index) => (
              <div key={index} className="bar-column">
                <span style={{ height: `${value}%` }} />
                <small>{['Seg', 'Ter', 'Qua', 'Qui', 'Sex', 'Sab'][index]}</small>
              </div>
            ))}
          </div>
        </div>
      </section>

      <section className="content-grid lower-grid">
        <div className="panel calendar-panel">
          <div className="panel-header">
            <h2>Agenda</h2>
            <span>{dateLabel}</span>
          </div>
          <div className="calendar-grid">
            {['Dom', 'Seg', 'Ter', 'Qua', 'Qui', 'Sex', 'Sáb'].map((day) => (
              <span key={day}>{day}</span>
            ))}
            {Array.from({ length: 35 }, (_, index) => (
              <button
                key={index}
                type="button"
                className={index === 10 ? 'calendar-day active' : 'calendar-day'}
              >
                {index + 1}
              </button>
            ))}
          </div>
        </div>

        <div className="panel calculator-panel">
          <h2>Calculadora</h2>
          <input aria-label="Resultado da calculadora" value={calculator} readOnly />
          <div className="calculator-buttons">
            {['7', '8', '9', '/', '4', '5', '6', '*', '1', '2', '3', '-', '0', '.', '=', '+'].map((value) => (
              <button key={value} onClick={() => handleCalculatorUse(value)}>{value}</button>
            ))}
            <button className="clear-button" onClick={() => handleCalculatorUse('C')}>C</button>
          </div>
        </div>
      </section>
    </main>
  )
}

export default App
